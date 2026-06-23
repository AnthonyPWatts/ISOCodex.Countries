param(
    [string] $Configuration = "Release",
    [string] $OutputDirectory = "artifacts/release-check"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$solution = Join-Path $repoRoot "ISOCodex.Countries.sln"
$outputPath = Join-Path $repoRoot $OutputDirectory
$propsPath = Join-Path $repoRoot "Directory.Build.props"
$packageProjectPath = Join-Path $repoRoot "src/ISOCodex.Countries/ISOCodex.Countries.csproj"

function Invoke-DotNet
{
    dotnet @args

    if ($LASTEXITCODE -ne 0)
    {
        throw "dotnet $($args -join ' ') failed with exit code $LASTEXITCODE."
    }
}

Set-Location $repoRoot

Write-Host "Cleaning $Configuration outputs..."
Invoke-DotNet clean $solution --configuration $Configuration

if (Test-Path $outputPath)
{
    Remove-Item -LiteralPath $outputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $outputPath | Out-Null

Write-Host "Restoring..."
Invoke-DotNet restore $solution

Write-Host "Building..."
Invoke-DotNet build $solution --configuration $Configuration --no-restore

Write-Host "Testing..."
Invoke-DotNet test $solution --configuration $Configuration --no-build

Write-Host "Packing..."
Invoke-DotNet pack $solution --configuration $Configuration --no-restore -o $outputPath

[xml] $props = Get-Content -LiteralPath $propsPath
$versionNode = $props.SelectSingleNode("/Project/PropertyGroup/Version")

if ($null -eq $versionNode)
{
    throw "Directory.Build.props does not contain a Version value."
}

$version = $versionNode.InnerText.Trim()

if ([string]::IsNullOrWhiteSpace($version))
{
    throw "Directory.Build.props does not contain a Version value."
}

[xml] $project = Get-Content -LiteralPath $packageProjectPath
$packageIdNode = $project.SelectSingleNode("/Project/PropertyGroup/PackageId")

if ($null -eq $packageIdNode)
{
    throw "The package project does not contain a PackageId value."
}

$packageId = $packageIdNode.InnerText.Trim()

if ([string]::IsNullOrWhiteSpace($packageId))
{
    throw "The package project does not contain a PackageId value."
}

$expectedPackageName = "$packageId.$version.nupkg"
$packages = @(Get-ChildItem -LiteralPath $outputPath -Filter "*.nupkg" | Sort-Object Name)
$packageNames = @($packages | ForEach-Object { $_.Name })

if ($packageNames.Count -ne 1)
{
    throw "Expected 1 package, but found $($packageNames.Count): $($packageNames -join ', ')"
}

if ($packageNames[0] -ne $expectedPackageName)
{
    throw "Expected package '$expectedPackageName', but found '$($packageNames[0])'."
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

$package = $packages[0]
Write-Host "Inspecting $($package.Name)..."

$archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)

try
{
    $entries = @($archive.Entries | ForEach-Object { $_.FullName })

    if ($entries -notcontains "README.md")
    {
        throw "$($package.Name) does not contain README.md."
    }

    $nuspec = $entries | Where-Object { $_ -like "*.nuspec" } | Select-Object -First 1

    if ($null -eq $nuspec)
    {
        throw "$($package.Name) does not contain a nuspec."
    }

    $netStandardLibrary = $entries |
        Where-Object { $_ -like "lib/netstandard2.1/*.dll" } |
        Select-Object -First 1

    if ($null -eq $netStandardLibrary)
    {
        throw "$($package.Name) does not contain a netstandard2.1 library."
    }

    $net8Library = $entries |
        Where-Object { $_ -like "lib/net8.0/*.dll" } |
        Select-Object -First 1

    if ($null -eq $net8Library)
    {
        throw "$($package.Name) does not contain a net8.0 library."
    }
}
finally
{
    $archive.Dispose()
}

Write-Host "Release check passed. Package is in $outputPath"
