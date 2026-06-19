# Data Sources

`ISOCodex.Countries` aligns conceptually with ISO 3166 country-code concepts, UN M49 numeric-code concepts where applicable, and CLDR-style display-name thinking where appropriate.

This project is not an official ISO product and does not claim ISO endorsement.

## Representative Seed Data

The current country seed is deliberately small, hand-curated, and representative. It proves package behaviour; it is not a redistributed official ISO table:

- `GB` / `GBR` / `826` - United Kingdom
- `US` / `USA` / `840` - United States
- `DE` / `DEU` / `276` - Germany
- `FR` / `FRA` / `250` - France
- `IE` / `IRL` / `372` - Ireland
- `AL` / `ALB` / `008` - Albania
- `CA` / `CAN` / `124` - Canada
- `AU` / `AUS` / `036` - Australia

Representative subdivision seed data currently includes `GB-ENG`, `GB-SCT`, `GB-WLS`, `GB-NIR`, `US-CA`, `CA-ON`, `AU-NSW`, and `IE-D`.

The JSON files in `data/` are source-aligned project seed files. The runtime package uses checked-in compiled seed data so it does not depend on loose external files after packaging.

The v1.0 data-source decision is tracked in [`data-strategy.md`](data-strategy.md) and [`decisions/0002-v1-data-scope.md`](decisions/0002-v1-data-scope.md).

## Data Version

- Identifier: `representative-seed-2026-06`
- Date checked: 2026-06-19
- Runtime exposure: `CountryDataVersion`

The data version identifies the checked-in representative seed posture. It is not an ISO publication identifier.

## Completeness Limits

The seed data is not a complete ISO 3166, ISO 3166-2, UN M49, or CLDR dataset. It is sufficient to prove the package architecture, validation behaviour, leading-zero numeric handling, registry lookup, representative subdivisions, and package workflow.

`GB` is the canonical ISO-style alpha-2 country code used by this package. `UK` is commonly encountered in real systems, but it is not silently treated as canonical `GB`.

`EU` and `ZZ` are syntactically valid alpha-2 shapes. The current package treats them as unknown because it does not yet model reserved, exceptional, or user-assigned code ranges as registry entries.

## Update Process

For small representative corrections:

1. Update `data/*.seed.json`.
2. Update `CountrySeedData`.
3. Update `CountryDataVersion` when the data posture or checked date changes.
4. Run the data drift and integrity tests.

For complete current-country expansion, document the source, version, date checked, redistribution position, and generation workflow before adding data.

## Future Expansion

Future data expansion should document:

- source and version checked,
- date checked,
- licensing or redistribution assumptions,
- whether the change affects canonical identifiers or display metadata,
- any known gaps or uncertainties.

Do not add copied data unless redistribution rights are clear.

## Proposing Corrections

Data corrections should include the source used, date checked, affected identifier, and whether the change affects canonical codes, display names, aliases, notes, or representative subdivision coverage.
