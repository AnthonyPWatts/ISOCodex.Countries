param(
    [string]$CldrVersion = "48.2",
    [string]$CldrTag = "release-48-2"
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$repoRoot = Split-Path -Parent $PSScriptRoot
$workRoot = Join-Path ([System.IO.Path]::GetTempPath()) "isocodex-countries-cldr"
$archivePath = Join-Path $workRoot "$CldrTag.zip"
$extractRoot = Join-Path $workRoot $CldrTag
$sourceRoot = Join-Path $extractRoot "cldr-$CldrTag"
$dataPath = Join-Path $repoRoot "data/countries.seed.json"
$codePath = Join-Path $repoRoot "src/ISOCodex.Countries/CountrySeedData.cs"

$excludedTerritories = @(
    "EU", # European Union; not an ISO 3166-1 country entry.
    "QO", # Outlying Oceania grouping.
    "XA", # CLDR pseudo-territory.
    "XB", # CLDR pseudo-territory.
    "XK", # Kosovo user-assigned code; not an ISO 3166-1 assigned entry.
    "ZZ"  # Unknown region placeholder.
)

$countryOverlays = @{
    "GB" = @{
        commonAliases = @("Britain", "Great Britain")
        notes = @(
            "GB is the canonical ISO-style alpha-2 country code used by this package.",
            "UK is commonly encountered but is not silently treated as canonical GB."
        )
    }
}

function ConvertTo-CSharpStringLiteral {
    param([string]$Value)

    return '"' + $Value.Replace('\', '\\').Replace('"', '\"') + '"'
}

function Format-CSharpStringArray {
    param([string[]]$Values)

    if ($Values.Count -eq 0) {
        return "null"
    }

    return "new[] { " + (($Values | ForEach-Object { ConvertTo-CSharpStringLiteral $_ }) -join ", ") + " }"
}

function Get-RequiredAttribute {
    param(
        [System.Xml.XmlElement]$Element,
        [string]$Name
    )

    $value = $Element.GetAttribute($Name)

    if ([string]::IsNullOrWhiteSpace($value)) {
        throw "CLDR element '$($Element.OuterXml)' is missing required attribute '$Name'."
    }

    return $value
}

New-Item -ItemType Directory -Force -Path $workRoot | Out-Null

if (-not (Test-Path -LiteralPath $archivePath)) {
    $archiveUri = "https://github.com/unicode-org/cldr/archive/refs/tags/$CldrTag.zip"
    Invoke-WebRequest -Uri $archiveUri -OutFile $archivePath
}

if (Test-Path -LiteralPath $extractRoot) {
    Remove-Item -LiteralPath $extractRoot -Recurse -Force
}

Expand-Archive -LiteralPath $archivePath -DestinationPath $extractRoot -Force

[xml]$supplementalData = Get-Content -Raw -Path (Join-Path $sourceRoot "common/supplemental/supplementalData.xml")
[xml]$supplementalMetadata = Get-Content -Raw -Path (Join-Path $sourceRoot "common/supplemental/supplementalMetadata.xml")
[xml]$englishData = Get-Content -Raw -Path (Join-Path $sourceRoot "common/main/en.xml")

$englishNames = @{}
foreach ($territory in $englishData.ldml.localeDisplayNames.territories.territory) {
    $type = $territory.type

    if ($type -and -not $territory.alt -and -not $englishNames.ContainsKey($type)) {
        $englishNames[$type] = $territory.InnerText
    }
}

$deprecatedTerritories = @{}
foreach ($alias in $supplementalMetadata.supplementalData.metadata.alias.territoryAlias) {
    $type = $alias.type

    if ($type -match "^[A-Z]{2}$" -and ($alias.reason -eq "deprecated" -or $alias.replacement)) {
        $deprecatedTerritories[$type] = $alias.replacement
    }
}

$records = @()
foreach ($territoryCode in $supplementalData.supplementalData.codeMappings.territoryCodes) {
    $alpha2 = Get-RequiredAttribute $territoryCode "type"
    $alpha3 = $territoryCode.GetAttribute("alpha3")
    $numeric = $territoryCode.GetAttribute("numeric")

    if ($alpha2 -notmatch "^[A-Z]{2}$" -or $alpha3 -notmatch "^[A-Z]{3}$" -or $numeric -notmatch "^\d{3}$") {
        continue
    }

    if ($deprecatedTerritories.ContainsKey($alpha2) -or $excludedTerritories -contains $alpha2) {
        continue
    }

    if (-not $englishNames.ContainsKey($alpha2)) {
        continue
    }

    $overlay = $countryOverlays[$alpha2]

    $record = [ordered]@{
        alpha2 = $alpha2
        alpha3 = $alpha3
        numeric = $numeric
        englishShortName = $englishNames[$alpha2]
        englishOfficialName = $null
        status = "Current"
    }

    if ($overlay -and $overlay.commonAliases) {
        $record.commonAliases = $overlay.commonAliases
    }

    if ($overlay -and $overlay.notes) {
        $record.notes = $overlay.notes
    }

    $records += [pscustomobject]$record
}

$records = @($records | Sort-Object alpha2)

if ($records.Count -ne 249) {
    throw "Expected 249 current CLDR territory records after filtering, found $($records.Count)."
}

$json = $records | ConvertTo-Json -Depth 8
Set-Content -Path $dataPath -Value ($json + [Environment]::NewLine) -Encoding UTF8

$countryLines = New-Object System.Collections.Generic.List[string]
foreach ($record in $records) {
    $aliases = if ($record.PSObject.Properties.Name -contains "commonAliases") { [string[]]$record.commonAliases } else { @() }
    $notes = if ($record.PSObject.Properties.Name -contains "notes") { [string[]]$record.notes } else { @() }

    $countryLines.Add("        new(")
    $countryLines.Add("            CountryAlpha2Code.Parse(" + (ConvertTo-CSharpStringLiteral $record.alpha2) + "),")
    $countryLines.Add("            CountryAlpha3Code.Parse(" + (ConvertTo-CSharpStringLiteral $record.alpha3) + "),")
    $countryLines.Add("            CountryNumericCode.Parse(" + (ConvertTo-CSharpStringLiteral $record.numeric) + "),")
    $countryLines.Add("            " + (ConvertTo-CSharpStringLiteral $record.englishShortName) + ",")
    $countryLines.Add("            null,")
    $countryLines.Add("            CountryEntryStatus.Current,")
    $countryLines.Add("            commonAliases: " + (Format-CSharpStringArray $aliases) + ",")
    $countryLines.Add("            notes: " + (Format-CSharpStringArray $notes) + "),")
}

$lastCountryLineIndex = $countryLines.Count - 1
$countryLines[$lastCountryLineIndex] = $countryLines[$lastCountryLineIndex].TrimEnd(",")

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
        new(CountrySubdivisionCode.Parse("GB-ENG"), CountryAlpha2Code.Parse("GB"), "England", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("GB-SCT"), CountryAlpha2Code.Parse("GB"), "Scotland", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("GB-WLS"), CountryAlpha2Code.Parse("GB"), "Wales", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("GB-NIR"), CountryAlpha2Code.Parse("GB"), "Northern Ireland", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("US-CA"), CountryAlpha2Code.Parse("US"), "California", null, CountrySubdivisionType.State),
        new(CountrySubdivisionCode.Parse("CA-ON"), CountryAlpha2Code.Parse("CA"), "Ontario", null, CountrySubdivisionType.Province),
        new(CountrySubdivisionCode.Parse("AU-NSW"), CountryAlpha2Code.Parse("AU"), "New South Wales", null, CountrySubdivisionType.State),
        new(CountrySubdivisionCode.Parse("IE-D"), CountryAlpha2Code.Parse("IE"), "Dublin", null, CountrySubdivisionType.County)
    }.AsReadOnly();
}
"@

Set-Content -Path $codePath -Value ($code.Replace("`r`n", "`n") + [Environment]::NewLine) -Encoding UTF8

Write-Output "Generated $($records.Count) country records from CLDR $CldrVersion ($CldrTag)."
