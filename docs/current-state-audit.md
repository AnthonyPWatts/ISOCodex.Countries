# Current State Audit

## Checked environment

- Branch: `main`
- Commit: release-candidate commit
- .NET SDK: `8.0.422`
- Audit date: 2026-06-20

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

Current country and territory entries are generated from Unicode CLDR 48.2. The registry contains 249 current ISO-style alpha-2, alpha-3, numeric, and English display-name entries for the selected v1 alpha scope.

Subdivision entries are generated from Unicode CLDR 48.2. The registry contains 5,027 regular subdivision code/name records across 200 countries for the selected v1 alpha scope.

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

The package version is explicitly `1.0.0-alpha` after adding CLDR-derived country and territory data.

CI restores, builds, tests, packs, runs the local package smoke test, and uploads package artifacts.

## Verification results

Current release-candidate verification:

| Command | Result |
|---|---|
| `dotnet restore` | Passed |
| `dotnet build --configuration Release` | Passed |
| `dotnet test --configuration Release` | Passed |
| `dotnet pack --configuration Release --output ./artifacts` | Passed |
| `./eng/smoke-test-package.ps1` | Passed |
| `dotnet run --project samples/CountryLookup.Console --configuration Release` | Passed |
| `dotnet run --project samples/CsvImport.Validation --configuration Release` | Passed |

The final local verification commands from `docs/release-gate.md` passed on 2026-06-20. The package smoke test checks package contents and local consumer installation.

## Gaps

- Country and territory data is complete for the selected CLDR-derived v1 alpha scope.
- Official ISO subdivision category/type enrichment is intentionally deferred.
- Reserved, exceptional, former, and user-assigned code modelling exists in enum shape only; the registry does not yet contain separate data for those states.
- JSON converters require manual registration and reject non-string tokens explicitly.
- Public API snapshot approval is lightweight reflection-based protection, not a full binary compatibility tool.

## Recommended next actions

1. Prepare `1.0.0-alpha` if the final release gate passes.
2. Review consumer feedback on the CLDR-derived data scope.
3. Review subdivision type and hierarchy enrichment only after a separate source decision.
