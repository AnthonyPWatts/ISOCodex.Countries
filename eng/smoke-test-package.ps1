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
