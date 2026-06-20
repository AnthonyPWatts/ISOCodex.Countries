param(
    [string]$CldrVersion = "48.2",
    [string]$CldrTag = "release-48-2",
    [string[]]$CompleteCountries
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$repoRoot = Split-Path -Parent $PSScriptRoot
$workRoot = Join-Path ([System.IO.Path]::GetTempPath()) "isocodex-countries-cldr"
$archivePath = Join-Path $workRoot "$CldrTag.zip"
$extractRoot = Join-Path $workRoot $CldrTag
$sourceRoot = Join-Path $extractRoot "cldr-$CldrTag"
$countryDataPath = Join-Path $repoRoot "data/countries.seed.json"
$subdivisionDataPath = Join-Path $repoRoot "data/subdivisions.seed.json"
$countryNameDataPath = Join-Path $repoRoot "data/country-names.seed.json"
$aliasDataPath = Join-Path $repoRoot "data/country-aliases.seed.json"
$codeElementDataPath = Join-Path $repoRoot "data/country-code-elements.seed.json"
$codePath = Join-Path $repoRoot "src/ISOCodex.Countries/CountrySeedData.cs"

$subdivisionTypeOverlays = @{
    "GB-ENG" = "Nation"
    "GB-SCT" = "Nation"
    "GB-WLS" = "Nation"
    "GB-NIR" = "Nation"
    "US-CA" = "State"
    "CA-ON" = "Province"
    "AU-NSW" = "State"
    "IE-D" = "County"
}

function ConvertTo-CSharpStringLiteral {
    param([string]$Value)

    $escaped = $Value.Replace('\', '\\').
        Replace('"', '\"').
        Replace("`r", '\r').
        Replace("`n", '\n').
        Replace("`t", '\t')

    return '"' + $escaped + '"'
}

function Format-CSharpNullableString {
    param([string]$Value)

    if ([string]::IsNullOrEmpty($Value)) {
        return "null"
    }

    return ConvertTo-CSharpStringLiteral $Value
}

function Format-CSharpStringArray {
    param([string[]]$Values)

    if ($Values.Count -eq 0) {
        return "null"
    }

    return "new[] { " + (($Values | ForEach-Object { ConvertTo-CSharpStringLiteral $_ }) -join ", ") + " }"
}

function Format-CSharpNullableAlpha2 {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return "null"
    }

    return "CountryAlpha2Code.Parse(" + (ConvertTo-CSharpStringLiteral $Value) + ")"
}

function Format-CSharpBoolean {
    param([bool]$Value)

    return $Value.ToString().ToLowerInvariant()
}

function Expand-CldrSubdivisionToken {
    param([string]$Token)

    if ($Token -notmatch "~") {
        return @($Token)
    }

    $parts = $Token -split "~", 2
    $start = $parts[0]
    $endTail = $parts[1]
    $prefixLength = $start.Length - $endTail.Length
    $prefix = $start.Substring(0, $prefixLength)
    $startTail = $start.Substring($prefixLength)

    if ($startTail -match "^\d+$" -and $endTail -match "^\d+$") {
        $width = $startTail.Length
        return ([int]$startTail)..([int]$endTail) | ForEach-Object { $prefix + $_.ToString("D$width") }
    }

    if ($startTail.Length -eq 1 -and $endTail.Length -eq 1) {
        return ([int][char]$startTail)..([int][char]$endTail) | ForEach-Object { $prefix + [char]$_ }
    }

    throw "Unsupported CLDR subdivision range token '$Token'."
}

function ConvertFrom-CldrSubdivisionId {
    param([string]$Id)

    if ($Id -notmatch "^[a-z]{2}[a-z0-9]{1,3}$") {
        throw "Unexpected CLDR subdivision id '$Id'."
    }

    return $Id.Substring(0, 2).ToUpperInvariant() + "-" + $Id.Substring(2).ToUpperInvariant()
}

function Get-ClrdSourceRoot {
    New-Item -ItemType Directory -Force -Path $workRoot | Out-Null

    if (-not (Test-Path -LiteralPath $archivePath)) {
        $archiveUri = "https://github.com/unicode-org/cldr/archive/refs/tags/$CldrTag.zip"
        Invoke-WebRequest -Uri $archiveUri -OutFile $archivePath
    }

    if (-not (Test-Path -LiteralPath $sourceRoot)) {
        if (Test-Path -LiteralPath $extractRoot) {
            Remove-Item -LiteralPath $extractRoot -Recurse -Force
        }

        Expand-Archive -LiteralPath $archivePath -DestinationPath $extractRoot -Force
    }

    return $sourceRoot
}

function Get-CldrSubdivisionRecords {
    $root = Get-ClrdSourceRoot

    [xml]$validityData = Get-Content -Raw -Path (Join-Path $root "common/validity/subdivision.xml")
    [xml]$englishData = Get-Content -Raw -Path (Join-Path $root "common/subdivisions/en.xml")

    $englishNames = @{}
    foreach ($subdivision in $englishData.ldml.localeDisplayNames.subdivisions.subdivision) {
        $type = $subdivision.type

        if ($type -and -not $subdivision.alt -and -not $englishNames.ContainsKey($type)) {
            $englishNames[$type] = $subdivision.InnerText
        }
    }

    $records = New-Object System.Collections.Generic.List[object]

    foreach ($idGroup in $validityData.supplementalData.idValidity.id) {
        if ($idGroup.type -ne "subdivision" -or $idGroup.idStatus -ne "regular") {
            continue
        }

        foreach ($token in ($idGroup.InnerText -split "\s+" | Where-Object { $_ })) {
            foreach ($id in (Expand-CldrSubdivisionToken $token)) {
                if (-not $englishNames.ContainsKey($id)) {
                    throw "CLDR subdivision '$id' has no English name in common/subdivisions/en.xml."
                }

                $code = ConvertFrom-CldrSubdivisionId $id
                $countryCode = $code.Substring(0, 2)
                $subdivisionType = if ($subdivisionTypeOverlays.ContainsKey($code)) { $subdivisionTypeOverlays[$code] } else { "Unknown" }

                $records.Add([pscustomobject][ordered]@{
                    code = $code
                    countryCode = $countryCode
                    englishName = $englishNames[$id]
                    localName = $null
                    type = $subdivisionType
                })
            }
        }
    }

    return @($records | Sort-Object code)
}

function New-CountryLines {
    param([object[]]$Countries)

    $lines = New-Object System.Collections.Generic.List[string]

    foreach ($country in $Countries) {
        $aliases = if ($country.PSObject.Properties.Name -contains "commonAliases") { [string[]]$country.commonAliases } else { @() }
        $notes = if ($country.PSObject.Properties.Name -contains "notes") { [string[]]$country.notes } else { @() }

        $lines.Add("        new(")
        $lines.Add("            CountryAlpha2Code.Parse(" + (ConvertTo-CSharpStringLiteral $country.alpha2) + "),")
        $lines.Add("            CountryAlpha3Code.Parse(" + (ConvertTo-CSharpStringLiteral $country.alpha3) + "),")
        $lines.Add("            CountryNumericCode.Parse(" + (ConvertTo-CSharpStringLiteral $country.numeric) + "),")
        $lines.Add("            " + (ConvertTo-CSharpStringLiteral $country.englishShortName) + ",")
        $lines.Add("            " + (Format-CSharpNullableString $country.englishOfficialName) + ",")
        $lines.Add("            CountryEntryStatus." + $country.status + ",")
        $lines.Add("            commonAliases: " + (Format-CSharpStringArray $aliases) + ",")
        $lines.Add("            notes: " + (Format-CSharpStringArray $notes) + "),")
    }

    $lines[$lines.Count - 1] = $lines[$lines.Count - 1].TrimEnd(",")
    return $lines
}

function New-SubdivisionLines {
    param([object[]]$Subdivisions)

    $lines = New-Object System.Collections.Generic.List[string]

    foreach ($subdivision in $Subdivisions) {
        $lines.Add(
            "        new(CountrySubdivisionCode.Parse(" + (ConvertTo-CSharpStringLiteral $subdivision.code) +
            "), CountryAlpha2Code.Parse(" + (ConvertTo-CSharpStringLiteral $subdivision.countryCode) +
            "), " + (ConvertTo-CSharpStringLiteral $subdivision.englishName) +
            ", " + (Format-CSharpNullableString $subdivision.localName) +
            ", CountrySubdivisionType." + $subdivision.type + "),")
    }

    $lines[$lines.Count - 1] = $lines[$lines.Count - 1].TrimEnd(",")
    return $lines
}

function New-CountryDisplayNameLines {
    param([object[]]$Names)

    $lines = New-Object System.Collections.Generic.List[string]

    foreach ($name in $Names) {
        $lines.Add(
            "        new(CountryAlpha2Code.Parse(" + (ConvertTo-CSharpStringLiteral $name.countryCode) +
            "), " + (ConvertTo-CSharpStringLiteral $name.languageTag) +
            ", " + (ConvertTo-CSharpStringLiteral $name.name) +
            ", CountryDisplayNameKind." + $name.kind +
            ", isEndonym: " + (Format-CSharpBoolean $name.isEndonym) +
            ", isRightToLeft: " + (Format-CSharpBoolean $name.isRightToLeft) + "),")
    }

    $lines[$lines.Count - 1] = $lines[$lines.Count - 1].TrimEnd(",")
    return $lines
}

function New-CountryAliasLines {
    param([object[]]$Aliases)

    $lines = New-Object System.Collections.Generic.List[string]

    foreach ($alias in $Aliases) {
        $lines.Add(
            "        new(" + (ConvertTo-CSharpStringLiteral $alias.alias) +
            ", " + (Format-CSharpNullableAlpha2 $alias.replacementCountryCode) +
            ", CountryAliasKind." + $alias.kind +
            ", " + (ConvertTo-CSharpStringLiteral $alias.source) +
            ", " + (Format-CSharpNullableString $alias.notes) + "),")
    }

    $lines[$lines.Count - 1] = $lines[$lines.Count - 1].TrimEnd(",")
    return $lines
}

function New-CountryCodeElementLines {
    param([object[]]$Elements)

    $lines = New-Object System.Collections.Generic.List[string]

    foreach ($element in $Elements) {
        $lines.Add(
            "        new(CountryAlpha2Code.Parse(" + (ConvertTo-CSharpStringLiteral $element.alpha2) +
            "), CountryCodeElementKind." + $element.kind +
            ", " + (ConvertTo-CSharpStringLiteral $element.displayName) +
            ", " + (ConvertTo-CSharpStringLiteral $element.source) +
            ", " + (Format-CSharpNullableString $element.notes) + "),")
    }

    $lines[$lines.Count - 1] = $lines[$lines.Count - 1].TrimEnd(",")
    return $lines
}

function Update-CountrySeedDataCode {
    param(
        [object[]]$Countries,
        [object[]]$Subdivisions,
        [object[]]$CountryDisplayNames,
        [object[]]$CountryAliases,
        [object[]]$CountryCodeElements
    )

    $countryLines = New-CountryLines $Countries
    $subdivisionLines = New-SubdivisionLines $Subdivisions
    $displayNameLines = New-CountryDisplayNameLines $CountryDisplayNames
    $aliasLines = New-CountryAliasLines $CountryAliases
    $codeElementLines = New-CountryCodeElementLines $CountryCodeElements

    $code = @"
namespace ISOCodex.Countries;

internal static class CountrySeedData
{
    public static IReadOnlyList<CountryInfo> Countries { get; } = new List<CountryInfo>
    {
$($countryLines -join [Environment]::NewLine)
    }.AsReadOnly();

    public static IReadOnlyList<CountrySubdivisionInfo> Subdivisions { get; } = new List<CountrySubdivisionInfo>
    {
$($subdivisionLines -join [Environment]::NewLine)
    }.AsReadOnly();

    public static IReadOnlyList<CountryDisplayName> CountryDisplayNames { get; } = new List<CountryDisplayName>
    {
$($displayNameLines -join [Environment]::NewLine)
    }.AsReadOnly();

    public static IReadOnlyList<CountryAliasInfo> CountryAliases { get; } = new List<CountryAliasInfo>
    {
$($aliasLines -join [Environment]::NewLine)
    }.AsReadOnly();

    public static IReadOnlyList<CountryCodeElementInfo> CountryCodeElements { get; } = new List<CountryCodeElementInfo>
    {
$($codeElementLines -join [Environment]::NewLine)
    }.AsReadOnly();
}
"@

    Set-Content -Path $codePath -Value ($code.Replace("`r`n", "`n") + [Environment]::NewLine) -Encoding UTF8
}

$countries = @((Get-Content -Raw -Path $countryDataPath | ConvertFrom-Json) | Sort-Object alpha2)
$currentSubdivisions = @((Get-Content -Raw -Path $subdivisionDataPath | ConvertFrom-Json) | Sort-Object code)
$countryDisplayNames = @((Get-Content -Raw -Path $countryNameDataPath | ConvertFrom-Json) | Sort-Object countryCode, languageTag, kind)
$countryAliases = @((Get-Content -Raw -Path $aliasDataPath | ConvertFrom-Json) | Sort-Object alias, replacementCountryCode, kind)
$countryCodeElements = @((Get-Content -Raw -Path $codeElementDataPath | ConvertFrom-Json) | Sort-Object alpha2)
$generatedSubdivisions = Get-CldrSubdivisionRecords

if ($generatedSubdivisions.Count -ne 5027) {
    throw "Expected 5027 CLDR subdivision records, found $($generatedSubdivisions.Count)."
}

$knownCountries = @{}
foreach ($country in $countries) {
    $knownCountries[$country.alpha2] = $true
}

$generatedCountryCodes = @($generatedSubdivisions | Select-Object -ExpandProperty countryCode -Unique | Sort-Object)
foreach ($countryCode in $generatedCountryCodes) {
    if (-not $knownCountries.ContainsKey($countryCode)) {
        throw "CLDR subdivision country '$countryCode' is not present in the country registry."
    }
}

if ($CompleteCountries.Count -eq 0) {
    $CompleteCountries = $generatedCountryCodes
}

$countriesToComplete = @($CompleteCountries | ForEach-Object { $_.ToUpperInvariant() } | Sort-Object -Unique)
foreach ($countryCode in $countriesToComplete) {
    if ($generatedCountryCodes -notcontains $countryCode) {
        throw "CLDR subdivision data contains no records for country '$countryCode'."
    }
}

$mergedSubdivisions = @(
    $currentSubdivisions | Where-Object { $countriesToComplete -notcontains $_.countryCode }
    $generatedSubdivisions | Where-Object { $countriesToComplete -contains $_.countryCode }
) | Sort-Object code

$json = $mergedSubdivisions | ConvertTo-Json -Depth 8
Set-Content -Path $subdivisionDataPath -Value ($json + [Environment]::NewLine) -Encoding UTF8

Update-CountrySeedDataCode `
    -Countries $countries `
    -Subdivisions $mergedSubdivisions `
    -CountryDisplayNames $countryDisplayNames `
    -CountryAliases $countryAliases `
    -CountryCodeElements $countryCodeElements

Write-Output "Generated subdivision seed data for $($countriesToComplete.Count) country/countries from CLDR $CldrVersion ($CldrTag)."
