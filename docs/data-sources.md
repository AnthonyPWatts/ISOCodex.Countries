# Data Sources

`ISOCodex.Countries` aligns conceptually with ISO 3166 country-code concepts, UN M49 numeric-code concepts where applicable, and CLDR-style display-name thinking where appropriate.

This project is not an official ISO product and does not claim ISO endorsement.

## Country And Territory Seed Data

The current country and territory seed is derived from Unicode CLDR 48.2. It includes 249 current ISO-style entries with:

- alpha-2 code;
- alpha-3 code;
- three-digit numeric code;
- English display name.

The generation source files are:

- `common/supplemental/supplementalData.xml`
- `common/supplemental/supplementalMetadata.xml`
- `common/main/en.xml`

The generator is `eng/update-country-seed-from-cldr.ps1`.

The generated registry excludes deprecated territory aliases, CLDR pseudo-territories, regional groupings, unknown-region placeholders, and user-assigned code elements that are not ISO 3166-1 assigned country entries. Examples include `EU`, `QO`, `XA`, `XB`, `XK`, and `ZZ`.

The package includes `THIRD-PARTY-NOTICES.md` for Unicode CLDR attribution and licence details.

## Country Display Names, Aliases, And Code Elements

Country display names are generated from selected Unicode CLDR 48.2 locale files:

- `common/main/en.xml`
- `common/main/de.xml`
- `common/main/el.xml`
- `common/main/ja.xml`
- `common/main/zh.xml` for the package's `zh-Hans` display-name entries
- `common/main/zh_Hant.xml`
- `common/main/ar.xml`
- `common/main/he.xml`
- `common/main/pt.xml`
- `common/main/fr.xml`
- `common/main/es.xml`

`common/main/pt_BR.xml` is part of the pinned fallback policy check, but CLDR 48.2 supplies no direct territory display names in that file; `pt-BR` lookups therefore fall back visibly to `pt`.

Generated display names are Unicode strings normalised to NFC. The package preserves source script exactly, including non-Latin and right-to-left text. It does not transliterate, strip accents, reverse right-to-left strings, or add manual directionality marks.

Display names are CLDR-derived labels. They are not official government names, ISO names, or a statement of geopolitical authority.

Endonym coverage is source-limited. The v1 package uses a small reviewed country-to-language mapping for obvious cases such as `DE`/`de`, `JP`/`ja`, `GR`/`el`, `CN`/`zh-Hans`, `SA`/`ar`, and `IL`/`he`. Other generated display names may be useful localised names but are not marked as endonyms unless the mapping deliberately says so.

Alias seed data comes from:

- reviewed repository overlays such as `Britain` and `Great Britain` for `GB`;
- CLDR `territoryAlias` entries in `common/supplemental/supplementalMetadata.xml`, including deprecated aliases such as `UK`.

Alias lookup is explicit through `CountryAliasRegistry`; `CountryRegistry.Lookup` does not resolve aliases or display names.

Special code elements are generated into `CountryCodeElementRegistry` for `EU`, `QO`, `XA`, `XB`, `XK`, and `ZZ`. These values are country-code-shaped elements, not current countries in this package.

## Subdivision Seed Data

The subdivision seed is derived from Unicode CLDR 48.2. It includes 5,027 regular subdivision entries across 200 countries.

The generation source files are:

- `common/validity/subdivision.xml`
- `common/subdivisions/en.xml`

The generator is `eng/update-subdivision-seed-from-cldr.ps1`.

The generated registry includes CLDR regular subdivision identifiers and English display names. It does not claim official ISO subdivision category/type wording. `CountrySubdivisionType` is `Unknown` unless a specific reviewed overlay exists.

## Data Version

- Identifier: `cldr-48.2-country-name-alias-subdivision-seed-2026-06`
- Date checked: 2026-06-19
- Runtime exposure: `CountryDataVersion`
- v1 country-data decision: [`decisions/0005-v1-alpha-country-data-scope.md`](decisions/0005-v1-alpha-country-data-scope.md)
- subdivision decision: [`decisions/0006-v1-alpha-subdivision-data-scope.md`](decisions/0006-v1-alpha-subdivision-data-scope.md)

The data version identifies the checked-in package data posture. It is not an ISO publication identifier.

## Completeness Limits

Country, territory, display-name, alias, special-code-element, and subdivision code/name coverage is complete for the selected CLDR-derived v1 scope.

The package does not currently model complete reserved code ranges, former-country entries, exceptional reservations, transitional reservations, user-assigned ranges, flags, calling codes, sanctions data, currencies, address formatting, geospatial data, online updates, official ISO subdivision categories, or subdivision containment hierarchy.

`GB` is the canonical ISO-style alpha-2 country code used by this package. `UK` is commonly encountered in real systems, but it is not silently treated as canonical `GB`.

`EU`, `QO`, `XA`, `XB`, `XK`, and `ZZ` are syntactically valid alpha-2 shapes. The current package returns `ReservedButNotCountry` for these known non-country or special-purpose codes because they are outside the selected current country and territory registry scope.

## Update Process

For CLDR country data updates:

1. Review the target CLDR release, licence, and source-file shape.
2. Update the pinned version/tag in `eng/update-country-seed-from-cldr.ps1` if needed.
3. Run `./eng/update-country-seed-from-cldr.ps1`.
4. Review `data/countries.seed.json`, `data/country-names.seed.json`, `data/country-aliases.seed.json`, `data/country-code-elements.seed.json`, and `CountrySeedData`.
5. Update `CountryDataVersion` when the source release or checked date changes.
6. Run data integrity, drift, package, and public API checks.

For CLDR subdivision data updates:

1. Review the target CLDR release, licence, and source-file shape.
2. Update the pinned version/tag in `eng/update-subdivision-seed-from-cldr.ps1` if needed.
3. Run `./eng/update-subdivision-seed-from-cldr.ps1`.
4. Review `data/subdivisions.seed.json` and `CountrySeedData`.
5. Update `CountryDataVersion` when the source release or checked date changes.
6. Run data integrity, drift, package, and public API checks.

Do not add copied official ISO tables unless redistribution rights are clear.

## Proposing Corrections

Data corrections should include the source used, date checked, affected identifier, and whether the change affects canonical codes, display names, aliases, notes, subdivision coverage, or the CLDR filter policy.
