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
$countryNameDataPath = Join-Path $repoRoot "data/country-names.seed.json"
$aliasDataPath = Join-Path $repoRoot "data/country-aliases.seed.json"
$codeElementDataPath = Join-Path $repoRoot "data/country-code-elements.seed.json"
$subdivisionDataPath = Join-Path $repoRoot "data/subdivisions.seed.json"
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

$displayNameLocales = @(
    @{ languageTag = "en"; sourceLocale = "en" },
    @{ languageTag = "de"; sourceLocale = "de" },
    @{ languageTag = "el"; sourceLocale = "el" },
    @{ languageTag = "ja"; sourceLocale = "ja" },
    @{ languageTag = "zh-Hans"; sourceLocale = "zh" },
    @{ languageTag = "zh-Hant"; sourceLocale = "zh-Hant" },
    @{ languageTag = "ar"; sourceLocale = "ar" },
    @{ languageTag = "he"; sourceLocale = "he" },
    @{ languageTag = "pt"; sourceLocale = "pt" },
    @{ languageTag = "pt-BR"; sourceLocale = "pt-BR" },
    @{ languageTag = "fr"; sourceLocale = "fr" },
    @{ languageTag = "es"; sourceLocale = "es" }
)

# This alpha uses a deliberately small reviewed mapping. The display-name data
# is CLDR-derived, but endonym flags are only set where the country/language
# association is clear enough for this package's first pass.
$endonymTagsByCountry = @{
    "BR" = @("pt")
    "CN" = @("zh-Hans")
    "DE" = @("de")
    "ES" = @("es")
    "FR" = @("fr")
    "GB" = @("en")
    "GR" = @("el")
    "IL" = @("he")
    "JP" = @("ja")
    "PT" = @("pt")
    "SA" = @("ar")
}

$codeElementRecords = @(
    [pscustomobject][ordered]@{
        alpha2 = "EU"
        kind = "RegionGrouping"
        displayName = "European Union"
        source = "Unicode CLDR release $CldrVersion common/main/en.xml territory display name"
        notes = "A region grouping, not a current country entry in this package."
    },
    [pscustomobject][ordered]@{
        alpha2 = "QO"
        kind = "RegionGrouping"
        displayName = "Outlying Oceania"
        source = "Unicode CLDR release $CldrVersion common/main/en.xml territory display name"
        notes = "A CLDR region grouping, not a current country entry in this package."
    },
    [pscustomobject][ordered]@{
        alpha2 = "XA"
        kind = "PseudoTerritory"
        displayName = "Pseudo-Accents"
        source = "Unicode CLDR release $CldrVersion common/main/en.xml territory display name"
        notes = "A CLDR pseudo-territory used for testing locale data."
    },
    [pscustomobject][ordered]@{
        alpha2 = "XB"
        kind = "PseudoTerritory"
        displayName = "Pseudo-Bidi"
        source = "Unicode CLDR release $CldrVersion common/main/en.xml territory display name"
        notes = "A CLDR pseudo-territory used for testing bidirectional locale data."
    },
    [pscustomobject][ordered]@{
        alpha2 = "XK"
        kind = "UserAssigned"
        displayName = "Kosovo"
        source = "Unicode CLDR release $CldrVersion common/main/en.xml territory display name"
        notes = "A commonly used user-assigned code; not treated as a current ISO 3166-1 country entry by this package."
    },
    [pscustomobject][ordered]@{
        alpha2 = "ZZ"
        kind = "UnknownRegion"
        displayName = "Unknown Region"
        source = "Unicode CLDR release $CldrVersion common/main/en.xml territory display name"
        notes = "A CLDR unknown-region placeholder."
    }
)

function ConvertTo-CSharpStringLiteral {
    param([string]$Value)

    $escaped = $Value.Replace('\', '\\').
        Replace('"', '\"').
        Replace("`r", '\r').
        Replace("`n", '\n').
        Replace("`t", '\t')

    return '"' + $escaped + '"'
}

function Format-CSharpStringArray {
    param([string[]]$Values)

    if ($Values.Count -eq 0) {
        return "null"
    }

    return "new[] { " + (($Values | ForEach-Object { ConvertTo-CSharpStringLiteral $_ }) -join ", ") + " }"
}

function Format-CSharpNullableString {
    param([string]$Value)

    if ([string]::IsNullOrEmpty($Value)) {
        return "null"
    }

    return ConvertTo-CSharpStringLiteral $Value
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

function Get-CldrSourceRoot {
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

function Get-CldrTerritoryNames {
    param(
        [string]$Root,
        [string]$LanguageTag
    )

    $fileName = $LanguageTag.Replace("-", "_") + ".xml"
    $filePath = Join-Path $Root "common/main/$fileName"

    if (-not (Test-Path -LiteralPath $filePath)) {
        throw "CLDR locale file '$fileName' was not found."
    }

    [xml]$localeData = Get-Content -Raw -Path $filePath
    $names = @{}

    foreach ($territory in $localeData.ldml.localeDisplayNames.territories.territory) {
        $type = $territory.type

        if ($type -and -not $territory.alt -and -not $names.ContainsKey($type)) {
            $names[$type] = $territory.InnerText.Normalize([System.Text.NormalizationForm]::FormC)
        }
    }

    return $names
}

function Test-IsRightToLeft {
    param([string]$LanguageTag)

    try {
        return [System.Globalization.CultureInfo]::GetCultureInfo($LanguageTag).TextInfo.IsRightToLeft
    }
    catch [System.Globalization.CultureNotFoundException] {
        $primary = ($LanguageTag -split "-", 2)[0]
        return @("ar", "fa", "he", "ur").Contains($primary)
    }
}

function Test-IsEndonym {
    param(
        [string]$CountryCode,
        [string]$LanguageTag
    )

    if (-not $endonymTagsByCountry.ContainsKey($CountryCode)) {
        return $false
    }

    return $endonymTagsByCountry[$CountryCode] -contains $LanguageTag
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

$sourceRoot = Get-CldrSourceRoot

[xml]$supplementalData = Get-Content -Raw -Path (Join-Path $sourceRoot "common/supplemental/supplementalData.xml")
[xml]$supplementalMetadata = Get-Content -Raw -Path (Join-Path $sourceRoot "common/supplemental/supplementalMetadata.xml")

$englishNames = Get-CldrTerritoryNames -Root $sourceRoot -LanguageTag "en"

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

$knownCountryCodes = @{}
foreach ($record in $records) {
    $knownCountryCodes[$record.alpha2] = $true
}

$displayNameRecords = @()
foreach ($locale in $displayNameLocales) {
    $languageTag = $locale.languageTag
    $sourceLocale = $locale.sourceLocale
    $names = Get-CldrTerritoryNames -Root $sourceRoot -LanguageTag $sourceLocale
    $isRightToLeft = Test-IsRightToLeft $languageTag

    foreach ($record in $records) {
        if (-not $names.ContainsKey($record.alpha2)) {
            continue
        }

        $displayNameRecords += [pscustomobject][ordered]@{
            countryCode = $record.alpha2
            languageTag = $languageTag
            name = $names[$record.alpha2]
            kind = "Short"
            isEndonym = (Test-IsEndonym -CountryCode $record.alpha2 -LanguageTag $languageTag)
            isRightToLeft = $isRightToLeft
        }
    }
}

$displayNameRecords = @($displayNameRecords | Sort-Object countryCode, languageTag, kind)

if ($displayNameRecords.Count -lt 2700) {
    throw "Expected at least 2700 generated country display names, found $($displayNameRecords.Count)."
}

$aliasRecords = @()
foreach ($record in $records) {
    $aliases = if ($record.PSObject.Properties.Name -contains "commonAliases") { [string[]]$record.commonAliases } else { @() }

    foreach ($commonAlias in $aliases) {
        $aliasRecords += [pscustomobject][ordered]@{
            alias = $commonAlias
            replacementCountryCode = $record.alpha2
            kind = "CommonName"
            source = "Repository overlay reviewed against common English usage"
            notes = "Alias lookup is explicit and does not affect canonical country-code lookup."
        }
    }
}

foreach ($alias in $supplementalMetadata.supplementalData.metadata.alias.territoryAlias) {
    $type = $alias.type
    $replacement = $alias.replacement

    if ($type -notmatch "^[A-Z]{2}$" -or [string]::IsNullOrWhiteSpace($replacement)) {
        continue
    }

    $replacementCodes = @($replacement -split "\s+" | Where-Object { $_ -match "^[A-Z]{2}$" -and $knownCountryCodes.ContainsKey($_) })

    foreach ($replacementCode in $replacementCodes) {
        $aliasRecords += [pscustomobject][ordered]@{
            alias = $type
            replacementCountryCode = $replacementCode
            kind = "DeprecatedCode"
            source = "Unicode CLDR release $CldrVersion common/supplemental/supplementalMetadata.xml territoryAlias"
            notes = "Deprecated or historical territory alias from CLDR. Alias lookup is explicit and may be ambiguous."
        }
    }
}

$aliasRecords = @(
    $aliasRecords |
        Sort-Object alias, replacementCountryCode, kind -Unique
)

$json = $records | ConvertTo-Json -Depth 8
Set-Content -Path $dataPath -Value ($json + [Environment]::NewLine) -Encoding UTF8

$nameJson = $displayNameRecords | ConvertTo-Json -Depth 8
Set-Content -Path $countryNameDataPath -Value ($nameJson + [Environment]::NewLine) -Encoding UTF8

$aliasJson = $aliasRecords | ConvertTo-Json -Depth 8
Set-Content -Path $aliasDataPath -Value ($aliasJson + [Environment]::NewLine) -Encoding UTF8

$codeElementJson = $codeElementRecords | ConvertTo-Json -Depth 8
Set-Content -Path $codeElementDataPath -Value ($codeElementJson + [Environment]::NewLine) -Encoding UTF8

$subdivisionRecords = @((Get-Content -Raw -Path $subdivisionDataPath | ConvertFrom-Json) | Sort-Object code)

Update-CountrySeedDataCode `
    -Countries $records `
    -Subdivisions $subdivisionRecords `
    -CountryDisplayNames $displayNameRecords `
    -CountryAliases $aliasRecords `
    -CountryCodeElements $codeElementRecords

Write-Output "Generated $($records.Count) country records, $($displayNameRecords.Count) display names, $($aliasRecords.Count) aliases, and $($codeElementRecords.Count) code elements from CLDR $CldrVersion ($CldrTag)."
