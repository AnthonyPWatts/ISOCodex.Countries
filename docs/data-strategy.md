# Data Strategy

## Current posture

`ISOCodex.Countries` targets `1.0.0-alpha` with CLDR-derived current country, territory, and regular subdivision seed data.

The package generates 249 current ISO-style alpha-2, alpha-3, numeric, and English display-name records plus 5,027 regular subdivision code/name records from Unicode CLDR 48.2. It is not a redistributed official ISO table, does not claim ISO endorsement, and is not a source of geopolitical authority.

## What v1 alpha promises

`1.0.0-alpha` promises a dependency-light foundation package:

- strongly typed country and subdivision code value objects;
- canonical parsing and syntax validation;
- current country and territory registry lookup for the selected CLDR-derived scope;
- subdivision registry lookup for the selected CLDR-derived regular subdivision scope;
- explicit success and failure results for mixed country-code input;
- manual `System.Text.Json` converter support;
- checked-in compiled data with no hidden runtime network calls;
- documented data limitations, source attribution, and version disclosure.

`1.0.0-alpha` does not promise localisation, flags, calling codes, currencies, sanctions data, address formatting, geospatial data, online updates, official ISO subdivision category names, subdivision hierarchy modelling, or authoritative modelling of reserved and former code ranges.

## Selected source path

Unicode CLDR 48.2 is the selected source for v1 alpha country, territory, and regular subdivision code/name coverage.

The generator uses:

- `common/supplemental/supplementalData.xml` for territory alpha-2, alpha-3, and numeric mappings;
- `common/supplemental/supplementalMetadata.xml` for deprecated territory aliases;
- `common/main/en.xml` for English territory display names.
- `common/validity/subdivision.xml` for regular subdivision identifiers;
- `common/subdivisions/en.xml` for English subdivision display names.

The generated package records exclude:

- deprecated territory aliases;
- CLDR pseudo-territories;
- regional groupings;
- unknown-region placeholders;
- user-assigned code elements that are not ISO 3166-1 assigned country entries.

The expected generated country/territory count is 249. The expected generated subdivision count is 5,027 across 200 countries.

## Redistribution considerations

The package derives records from CLDR data under Unicode License v3 and includes `THIRD-PARTY-NOTICES.md` in the repository and NuGet package.

The project still avoids copying complete official ISO tables. ISO remains the authoritative standard, but this package uses CLDR as the redistribution-safe checked source for the v1 alpha country registry.

## Update workflow

For CLDR country data:

1. Review the CLDR release and licence.
2. Update `eng/update-country-seed-from-cldr.ps1` if the pinned source tag changes.
3. Regenerate `data/countries.seed.json` and compiled `CountrySeedData`.
4. Verify the generated count and selected exclusions.
5. Update `CountryDataVersion`.
6. Run data integrity, drift, package, and public API checks.

For subdivision data:

1. Review the CLDR release and licence.
2. Update `eng/update-subdivision-seed-from-cldr.ps1` if the pinned source tag changes.
3. Regenerate `data/subdivisions.seed.json` and compiled `CountrySeedData`.
4. Verify the generated count, country count, and selected type overlays.
5. Update `CountryDataVersion`.
6. Run data integrity, drift, package, and public API checks.

## Testing requirements

Tests should continue to prove:

- generated country count is 249;
- generated subdivision count is 5,027 across 200 countries;
- duplicate prevention;
- syntax validity;
- no empty display metadata, aliases, or notes;
- alias metadata is not silent canonical lookup;
- JSON seed and compiled seed alignment;
- CLDR source attribution remains documented;
- selected edge cases such as `AX`, `BV`, and `HM` are present;
- subdivision edge cases such as `AD-02`, `GB-ENG`, and `US-CA` are present;
- excluded values such as `EU`, `QO`, `XA`, `XB`, `XK`, and `ZZ` parse as alpha-2 but return `ReservedButNotCountry` from mixed lookup until a richer reserved or special-code model is deliberately added.

## Current recommendation

Prepare `1.0.0-alpha` as the next release candidate if the final release gate passes. Keep NuGet publishing separate and intentional.
