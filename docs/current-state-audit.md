# Current State Audit

## Checked environment

- Branch: `main`
- Commit: release-candidate working tree
- .NET SDK: `8.0.422`
- Audit date: 2026-06-19

## Projects and target frameworks

The solution contains:

- `src/ISOCodex.Countries/ISOCodex.Countries.csproj`
  - Targets `netstandard2.1;net8.0`
  - Packable package project
- `tests/ISOCodex.Countries.Tests/ISOCodex.Countries.Tests.csproj`
  - Targets `net8.0`
  - xUnit test project
- `samples/CountryLookup.Console/CountryLookup.Console.csproj`
  - Targets `net8.0`
- `samples/CsvImport.Validation/CsvImport.Validation.csproj`
  - Targets `net8.0`

## Public API summary

Public value objects found:

- `CountryAlpha2Code`
- `CountryAlpha3Code`
- `CountryNumericCode`
- `CountrySubdivisionCode`

Public metadata and result types found:

- `CountryInfo`
- `CountrySubdivisionInfo`
- `CountryCodeLookupResult`
- `CountryCodeValidationIssue`
- `CountryDataVersion`

Public enum types found:

- `CountryEntryStatus`
- `CountrySubdivisionType`
- `CountryCodeKind`
- `CountryCodeLookupFailureReason`

Public registries found:

- `CountryRegistry`
- `CountrySubdivisionRegistry`

JSON converter types found:

- `CountryAlpha2CodeJsonConverter`
- `CountryAlpha3CodeJsonConverter`
- `CountryNumericCodeJsonConverter`
- `CountrySubdivisionCodeJsonConverter`

## Registry and data coverage

The country registry uses compiled seed data from `CountrySeedData`.

Current representative country entries:

- `GB` / `GBR` / `826`
- `US` / `USA` / `840`
- `DE` / `DEU` / `276`
- `FR` / `FRA` / `250`
- `IE` / `IRL` / `372`
- `AL` / `ALB` / `008`
- `CA` / `CAN` / `124`
- `AU` / `AUS` / `036`

Representative subdivision entries include `GB-ENG`, `GB-SCT`, `GB-WLS`, `GB-NIR`, `US-CA`, `CA-ON`, `AU-NSW`, and `IE-D`.

The JSON seed files in `data/` mirror the compiled seed data and are now protected by tests.

## Tests and samples

Tests found:

- value-object syntax and normalisation tests
- registry lookup tests
- JSON converter tests
- data integrity tests
- public API ergonomics tests
- JSON/compiled seed drift tests
- public API snapshot test

Samples found:

- `CountryLookup.Console`
- `CsvImport.Validation`

## Documentation

Existing documentation found:

- `README.md`
- `CHANGELOG.md`
- `docs/data-sources.md`
- `docs/design-notes.md`
- `docs/versioning.md`
- `docs/decisions/0001-project-intent.md`

Additional hardening documentation has been added for current state, API review, definition of done, release gates, and roadmap.

## Package and CI

Package metadata includes package ID, title, description, author, repository URL, project URL, MIT licence expression, tags, README packaging, deterministic builds, Source Link, and XML documentation generation for packable projects.

The package version is explicitly pre-1.0 while the data remains representative rather than complete.

CI restores, builds, tests, packs, runs the local package smoke test, and uploads package artifacts.

## Verification results

Current release-candidate verification:

| Command | Result |
|---|---|
| `dotnet restore` | Passed |
| `dotnet build --configuration Release` | Passed |
| `dotnet test --configuration Release` | Passed, 116 tests |
| `dotnet pack --configuration Release --output ./artifacts` | Passed |

After hardening, run the final verification commands from `docs/release-gate.md` before any release. The package smoke test now also checks package contents and local consumer installation.

## Gaps

- Country data is representative only, not complete current-country coverage.
- Full ISO 3166-2 subdivision coverage is intentionally deferred.
- Reserved, exceptional, former, and user-assigned code modelling exists in enum shape only; the registry does not yet contain separate data for those states.
- JSON converters require manual registration and reject non-string tokens explicitly.
- Public API snapshot approval is lightweight reflection-based protection, not a full binary compatibility tool.

## Recommended next actions

1. Keep the package as v0.x while data-source and completeness questions remain open.
2. Decide whether v1.0 requires complete redistribution-safe current-country data or an explicitly representative scope.
3. If complete data is chosen, document source, version, date checked, and redistribution basis before expanding seed data.
4. Keep subdivision expansion out of the core package until the data strategy is proven.
