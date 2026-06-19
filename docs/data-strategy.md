# Data Strategy

## Current posture

`ISOCodex.Countries` currently ships a small hand-curated representative seed dataset. It proves the package API, validation behaviour, numeric leading-zero handling, registry lookup, subdivision modelling, data drift tests, packaging, and documentation workflow.

The seed is not a redistributed official ISO table, not complete ISO 3166 coverage, and not a source of geopolitical authority.

## What v0.x promises

v0.x promises a dependency-light foundation package:

- strongly typed country and subdivision code value objects;
- canonical parsing and syntax validation;
- representative registry lookup;
- explicit success and failure results for mixed country-code input;
- manual `System.Text.Json` converter support;
- checked-in compiled data with no hidden runtime network calls;
- documented data limitations and version disclosure.

v0.x does not promise complete current-country coverage, complete subdivision coverage, localisation, flags, calling codes, currencies, sanctions data, or address formatting.

## What v1.0 could promise

Before `1.0.0`, choose one of these positions:

1. Complete current-country coverage for the claimed package scope.
2. Stable representative foundation as the v1.0 contract.
3. Remain v0.x until complete data sourcing is solved.

The current recommendation is option 3. A package named `Countries` creates a reasonable consumer expectation of broad current-country coverage, so publishing `1.0.0` with representative data only would need an explicit product decision.

## Data-source requirements

Any complete-data path must identify:

- source authority;
- snapshot or release version;
- date checked;
- redistribution and licence position;
- fields imported and fields intentionally not imported;
- update workflow;
- generated-output ownership;
- tests that prove completeness for the selected source.

Do not copy complete official tables into this repository until redistribution rights and attribution obligations are clear.

## Candidate source paths

### ISO 3166 material

ISO 3166 is the authoritative code standard for country and subdivision code elements. ISO states that the Online Browsing Platform is the up-to-date access path and that the Country Codes Collection provides downloadable official lists in integration-friendly formats.

This is authoritative, but redistribution rights must be checked before copying full tables or generated derivatives into an MIT NuGet package.

Useful references:

- <https://www.iso.org/iso-3166-country-codes.html>
- <https://www.iso.org/obp/>

### UN M49 and UN terminology material

UN M49 is useful for numeric codes and country-or-area concepts. UNSD describes M49 as a statistical classification and publishes country or area names, M49 numeric codes, and ISO alpha-3 codes.

UN reuse terms need care. UN rights guidance says permission is required to reuse content from UN online platforms and statistical databases beyond allowed excerpts. Treat UN statistical database content as unsuitable for direct redistribution until permission or a clearer licence path is established.

Useful references:

- <https://unstats.un.org/unsd/methodology/m49/>
- <https://unstats.un.org/unsd/classifications/Family/Detail/12>
- <https://shop.un.org/rights-permissions>

### Unicode CLDR territory data

CLDR is useful for locale-oriented territory display names and stable release snapshots. Unicode states that CLDR releases are stable, and Unicode Data Files are generally subject to the Unicode licence unless a release states otherwise.

CLDR may be a good candidate for display-name and territory metadata, but it is not a drop-in ISO 3166 source. A future implementation must verify the exact files, release version, licence obligations, and mapping from CLDR territory identifiers to this package's country model.

Useful references:

- <https://cldr.unicode.org/index/downloads>
- <https://unicode.org/copyright.html>

### Hand-curated seed expansion

Hand-curated additions are acceptable for v0.x representative examples and consumer-driven corrections. They are not a good basis for claiming complete v1.0 coverage unless the source and review workflow become systematic.

### Consumer-driven incremental additions

Consumer-driven additions work for practical Addressing-style scenarios, but they do not solve the expectation that a `Countries` package should know current countries broadly.

## Redistribution considerations

The project should distinguish facts, code elements, source wording, tables, and curated package records. Even where individual codes are widely used, copying a complete source table or source wording may create licence and attribution obligations.

The safest future path is a pinned source file, a normalised project-owned JSON shape, generated compiled seed data, and tests that prove the generated output matches the pinned source. The source file and generated output should carry clear attribution and licence notes before any complete dataset is committed.

## Update workflow

For the current representative seed:

1. Update `data/*.seed.json`.
2. Update `CountrySeedData`.
3. Update `CountryDataVersion` when the data posture or checked date changes.
4. Run data integrity and drift tests.

For a future complete dataset:

1. Pin the selected source snapshot.
2. Normalise into a project-owned JSON schema.
3. Generate compiled C# seed data deterministically.
4. Fail generation on duplicate alpha-2, alpha-3, or numeric codes.
5. Preserve numeric leading zeroes.
6. Update `CountryDataVersion`.
7. Run completeness, drift, package, and public API checks.

## Testing requirements

Current tests should continue to prove:

- duplicate prevention;
- syntax validity;
- no empty display metadata, aliases, or notes;
- alias metadata is not silent canonical lookup;
- JSON seed and compiled seed alignment;
- representative-data documentation remains explicit.

If complete current-country coverage is claimed later, add tests for the expected record count, source snapshot identity, known awkward examples, and selected-source completeness.

## Current recommendation

Prepare and publish `0.1.0` as a representative v0.x foundation if the final release gate passes. Keep `1.0.0` blocked until the data-scope decision is made.

## Decision record required before v1.0

`docs/decisions/0002-v1-data-scope.md` records the pending decision. Do not change the package to `1.0.0` until that decision is accepted.
