param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$repoRoot = Split-Path -Parent $PSScriptRoot
$artifacts = Join-Path $repoRoot "artifacts"
$dotnetHome = Join-Path $artifacts ".dotnet-home"
$consumer = Join-Path $artifacts "PackageSmokeConsumer"
$project = Join-Path $consumer "PackageSmokeConsumer.csproj"
$program = Join-Path $consumer "Program.cs"
$packageProject = Join-Path $repoRoot "src/ISOCodex.Countries/ISOCodex.Countries.csproj"

New-Item -ItemType Directory -Force -Path $artifacts | Out-Null
dotnet pack $packageProject --configuration $Configuration --output $artifacts

$packageVersion = dotnet msbuild $packageProject -getProperty:Version
$packageVersion = $packageVersion.Trim()
$packageId = dotnet msbuild $packageProject -getProperty:PackageId
$packageId = $packageId.Trim()
$packagePath = Join-Path $artifacts "$packageId.$packageVersion.nupkg"

if (-not (Test-Path -LiteralPath $packagePath)) {
    throw "Expected package was not created: $packagePath"
}

$matchingPackages = @(Get-ChildItem -Path $artifacts -Filter "$packageId.$packageVersion*.nupkg")
if ($matchingPackages.Count -ne 1) {
    throw "Expected exactly one $packageId $packageVersion package, found $($matchingPackages.Count)."
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

function Test-PackageEntry {
    param(
        [string[]]$Entries,
        [string]$Expected
    )

    if ($Entries -notcontains $Expected) {
        throw "Package is missing expected entry '$Expected'."
    }
}

$package = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    $entries = @($package.Entries | ForEach-Object { $_.FullName })

    Test-PackageEntry $entries "README.md"
    Test-PackageEntry $entries "$packageId.nuspec"
    Test-PackageEntry $entries "lib/netstandard2.1/$packageId.dll"
    Test-PackageEntry $entries "lib/netstandard2.1/$packageId.xml"
    Test-PackageEntry $entries "lib/net8.0/$packageId.dll"
    Test-PackageEntry $entries "lib/net8.0/$packageId.xml"

    $looseRuntimeData = @($entries | Where-Object { $_ -like "data/*.json" -or $_ -like "content*/data/*.json" })
    if ($looseRuntimeData.Count -gt 0) {
        throw "Package contains loose runtime data files: $($looseRuntimeData -join ', ')"
    }

    $nuspecEntry = $package.Entries | Where-Object { $_.FullName -eq "$packageId.nuspec" } | Select-Object -First 1
    $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())
    try {
        $nuspec = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }

    foreach ($requiredText in @("<license type=`"expression`">MIT</license>", "<readme>README.md</readme>", "<repository type=`"git`" url=`"https://github.com/AnthonyPWatts/ISOCodex.Countries`"")) {
        if (-not $nuspec.Contains($requiredText)) {
            throw "Package nuspec is missing expected metadata: $requiredText"
        }
    }
}
finally {
    $package.Dispose()
}

New-Item -ItemType Directory -Force -Path $dotnetHome | Out-Null
$env:DOTNET_CLI_HOME = $dotnetHome
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

dotnet new console -n PackageSmokeConsumer -o $consumer --force

$projectText = Get-Content -Raw -Path $project
$projectText = $projectText.Replace(
    "    <Nullable>enable</Nullable>",
    "    <Nullable>enable</Nullable>`r`n    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>")
Set-Content -Path $project -Value $projectText -Encoding UTF8

dotnet add $project package ISOCodex.Countries --version $packageVersion --source $artifacts

@"
using ISOCodex.Countries;

CountryAlpha2Code code = CountryAlpha2Code.Parse("GB");
CountryInfo country = CountryRegistry.GetByAlpha2(code);

if (country.Alpha3.Value != "GBR")
{
    throw new InvalidOperationException("Package smoke lookup returned the wrong country.");
}

Console.WriteLine(country.Alpha2 + " / " + country.Alpha3 + " / " + country.Numeric + " - " + country.EnglishShortName);
"@ | Set-Content -Path $program -Encoding UTF8

dotnet run --project $project --configuration $Configuration
