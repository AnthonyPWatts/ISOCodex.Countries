# Data Sources

`ISOCodex.Countries` aligns conceptually with ISO 3166 country-code concepts, UN M49 numeric-code concepts where applicable, and CLDR-style display-name thinking where appropriate.

This project is not an official ISO product and does not claim ISO endorsement.

## First-Pass Seed Data

The first-pass country seed is deliberately small and representative:

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

## Completeness Limits

The seed data is not a complete ISO 3166, ISO 3166-2, UN M49, or CLDR dataset. It is sufficient to prove the package architecture, validation behaviour, leading-zero numeric handling, registry lookup, representative subdivisions, and package workflow.

`GB` is the canonical ISO-style alpha-2 country code used by this package. `UK` is commonly encountered in real systems, but it is not silently treated as canonical `GB`.

## Future Expansion

Future data expansion should document:

- source and version checked,
- date checked,
- licensing or redistribution assumptions,
- whether the change affects canonical identifiers or display metadata,
- any known gaps or uncertainties.

Do not add copied data unless redistribution rights are clear.
