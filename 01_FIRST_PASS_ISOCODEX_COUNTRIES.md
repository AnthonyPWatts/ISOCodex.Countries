# Codex First Development Pass: ISOCodex.Countries

## Purpose

This is the **first real development task** for the new `ISOCodex.Countries` repository.

Assume the bootstrap task has already created the repository scaffold.

Build a working, tested, packageable .NET foundation for country and country-code handling.

This is not a data-completeness task. It is a foundation task.

## Product definition

`ISOCodex.Countries` gives .NET developers strongly typed country, country-code, subdivision, and jurisdiction metadata for normal business applications.

It should feel like the same family as `ISOCodex.Addressing` and `ISOCodex.Currency`: practical, standards-aware, domain-oriented, and boringly useful.

## Core intent

The package should help application developers stop doing this:

```csharp
string country = "GB";
```

and instead do this:

```csharp
CountryAlpha2Code countryCode = CountryAlpha2Code.Parse("GB");
CountryInfo country = CountryRegistry.GetByAlpha2(countryCode);
```

It should provide:

- value objects,
- validation,
- canonical formatting,
- registry lookup,
- conversion between alpha-2, alpha-3, and numeric codes,
- structured failure information,
- JSON support,
- seed data infrastructure,
- tests,
- docs,
- samples,
- package/CI polish.

## Non-negotiable non-goals

Do not build:

- a geopolitical authority,
- a sanctions/compliance database,
- a real-time political data service,
- flags/emoji support,
- calling-code support,
- currency support,
- address formatting,
- full localisation,
- full ISO 3166-2 global subdivision data,
- hidden network calls,
- NuGet publishing.

Do not claim official ISO endorsement.

## Work style

Work in phases and create sensible commits/checkpoints if available.

Do not spend excessive time trying to solve official ISO data redistribution. If uncertain:

1. implement the architecture,
2. use a small, safe seed dataset,
3. document the limitation clearly,
4. leave focused TODOs.

Prefer a complete, tested, honest foundation over an unfinished attempt at total global coverage.

## Target solution structure

Create the actual .NET solution and projects.

Preferred structure:

```text
ISOCodex.Countries.sln
src/
  ISOCodex.Countries/
    ISOCodex.Countries.csproj
tests/
  ISOCodex.Countries.Tests/
    ISOCodex.Countries.Tests.csproj
samples/
  CountryLookup.Console/
    CountryLookup.Console.csproj
  CsvImport.Validation/
    CsvImport.Validation.csproj
data/
  countries.seed.json
  subdivisions.seed.json
```

Keep JSON converters in the core project for the first pass if that stays clean. Do not create a separate JSON package unless there is a strong reason.

## Target frameworks

Use:

```xml
<TargetFrameworks>netstandard2.0;net8.0</TargetFrameworks>
```

For the core library unless this causes unnecessary pain.

Test and sample projects can target `net8.0`.

If `netstandard2.0` creates friction with modern APIs, keep the core implementation compatible rather than dropping it casually.

## Core public types

Implement the following minimum public API.

### `CountryAlpha2Code`

Immutable value object.

Requirements:

- canonical uppercase storage,
- accepts case-insensitive input,
- rejects null/empty/whitespace,
- rejects wrong length,
- rejects non-ASCII letters,
- value equality,
- `ToString()` returns canonical uppercase,
- implements useful equality/comparison interfaces where appropriate.

Required methods:

```csharp
public static CountryAlpha2Code Parse(string value);
public static bool TryParse(string? value, out CountryAlpha2Code code);
public static bool IsValidSyntax(string? value);
```

If useful, add a richer parse method returning a validation issue.

### `CountryAlpha3Code`

Same as alpha-2, but exactly three ASCII letters.

### `CountryNumericCode`

Immutable value object for three-digit numeric/M49-style codes.

Requirements:

- canonical string storage,
- preserves leading zeroes,
- accepts string input,
- optional `FromInt32(int value)` if done safely,
- rejects wrong length,
- rejects non-digits,
- `ToString()` returns exactly three digits.

### `CountrySubdivisionCode`

Immutable value object for ISO 3166-2-style subdivision codes.

Requirements:

- canonical uppercase storage,
- expected shape: `{alpha2}-{subdivisionPart}`,
- validates alpha-2 prefix syntax,
- validates subdivision part as alphanumeric, 1-3 characters for first pass,
- exposes:
  - `CountryAlpha2Code CountryCode`
  - `string SubdivisionPart`
  - `string Value`

Do not treat syntactically valid subdivision codes as known unless they exist in the seed registry.

### `CountryInfo`

Represents a known country/territory entry.

Suggested shape:

```csharp
public sealed class CountryInfo
{
    public CountryAlpha2Code Alpha2 { get; }
    public CountryAlpha3Code Alpha3 { get; }
    public CountryNumericCode Numeric { get; }
    public string EnglishShortName { get; }
    public string? EnglishOfficialName { get; }
    public CountryEntryStatus Status { get; }
    public IReadOnlyList<string> CommonAliases { get; }
    public IReadOnlyList<string> Notes { get; }
}
```

Keep it immutable from the consumer perspective.

### `CountryEntryStatus`

Implement only statuses used in the first pass, but leave room for later.

Suggested:

```csharp
public enum CountryEntryStatus
{
    Current = 0,
    Former = 1,
    Reserved = 2,
    ExceptionallyReserved = 3,
    TransitionallyReserved = 4,
    UserAssigned = 5,
    Unknown = 99
}
```

If only `Current` is used initially, document the others or trim them.

### `CountrySubdivisionInfo`

Suggested shape:

```csharp
public sealed class CountrySubdivisionInfo
{
    public CountrySubdivisionCode Code { get; }
    public CountryAlpha2Code CountryCode { get; }
    public string EnglishName { get; }
    public string? LocalName { get; }
    public CountrySubdivisionType Type { get; }
}
```

### `CountrySubdivisionType`

Suggested:

```csharp
public enum CountrySubdivisionType
{
    Unknown = 0,
    State,
    Province,
    Territory,
    Region,
    County,
    District,
    Department,
    Municipality,
    Parish,
    CouncilArea,
    Nation,
    Other
}
```

### Validation issue model

Add small structured validation/failure types.

Example:

```csharp
public sealed class CountryCodeValidationIssue
{
    public string Code { get; }
    public string Message { get; }
    public string? Input { get; }
}
```

Stable issue-code examples:

- `country.alpha2.empty`
- `country.alpha2.invalid_length`
- `country.alpha2.invalid_characters`
- `country.alpha2.unknown`
- `country.alpha3.empty`
- `country.alpha3.invalid_length`
- `country.alpha3.invalid_characters`
- `country.numeric.empty`
- `country.numeric.invalid_length`
- `country.numeric.invalid_characters`
- `country.subdivision.invalid_format`
- `country.subdivision.unknown`

Keep this simple. Do not build a whole validation framework.

## Registry API

Implement a discoverable static registry.

Preferred entry point:

```csharp
public static class CountryRegistry
{
    public static IReadOnlyList<CountryInfo> All { get; }

    public static CountryInfo GetByAlpha2(CountryAlpha2Code code);
    public static CountryInfo GetByAlpha3(CountryAlpha3Code code);
    public static CountryInfo GetByNumeric(CountryNumericCode code);

    public static bool TryGetByAlpha2(CountryAlpha2Code code, out CountryInfo? country);
    public static bool TryGetByAlpha3(CountryAlpha3Code code, out CountryInfo? country);
    public static bool TryGetByNumeric(CountryNumericCode code, out CountryInfo? country);

    public static bool TryGetByAlpha2(string? value, out CountryInfo? country);
    public static bool TryGetByAlpha3(string? value, out CountryInfo? country);
    public static bool TryGetByNumeric(string? value, out CountryInfo? country);

    public static CountryCodeLookupResult Lookup(string? value);
}
```

`GetBy...` methods should throw clear exceptions for unknown known-good syntax.

`TryGetBy...` methods should not throw for normal invalid/unknown input.

### `CountryCodeLookupResult`

Implement a structured lookup result for mixed input.

Suggested shape:

```csharp
public sealed class CountryCodeLookupResult
{
    public bool Success { get; }
    public CountryInfo? Country { get; }
    public CountryCodeKind? DetectedKind { get; }
    public CountryCodeLookupFailureReason? FailureReason { get; }
    public string? NormalizedInput { get; }
}
```

Suggested enums:

```csharp
public enum CountryCodeKind
{
    Alpha2,
    Alpha3,
    Numeric
}

public enum CountryCodeLookupFailureReason
{
    Empty,
    InvalidSyntax,
    Ambiguous,
    Unknown,
    ReservedButNotCountry,
    Unsupported
}
```

Keep mixed lookup behaviour deterministic and documented.

## JSON support

Use `System.Text.Json`.

Implement converters for:

- `CountryAlpha2Code`
- `CountryAlpha3Code`
- `CountryNumericCode`
- `CountrySubdivisionCode`

Converters should:

- serialise to canonical strings,
- reject invalid values on deserialisation with clear `JsonException`s,
- be tested.

If automatic converter discovery is not possible without package overhead, document how to add converters manually.

## Seed data

Create:

```text
data/countries.seed.json
data/subdivisions.seed.json
```

Then either:

1. load from embedded resources at runtime, or
2. generate a checked-in static data class from the seed files.

Choose the simpler robust approach.

The runtime package should not depend on loose external files being present after packaging.

### Minimum country seed data

Include at least:

- `GB` / `GBR` / `826` — United Kingdom
- `US` / `USA` / `840` — United States
- `DE` / `DEU` / `276` — Germany
- `FR` / `FRA` / `250` — France
- `IE` / `IRL` / `372` — Ireland
- `AL` / `ALB` / `008` — Albania, specifically to prove leading-zero numeric handling
- `CA` / `CAN` / `124` — Canada
- `AU` / `AUS` / `036` — Australia

Include notes for GB/UK:

- `GB` is the canonical ISO-style alpha-2 country code used by the package.
- `UK` is commonly encountered but must not be silently treated as canonical `GB`.
- If alias handling is included, it must be explicit and documented.

Do not add fake countries to make tests easier.

### Minimum subdivision seed data

Include representative subdivisions only:

- `GB-ENG` — England — Nation
- `GB-SCT` — Scotland — Nation
- `GB-WLS` — Wales — Nation
- `GB-NIR` — Northern Ireland — Nation
- `US-CA` — California — State
- `CA-ON` — Ontario — Province
- `AU-NSW` — New South Wales — State
- `IE-D` — Dublin — County or Other, depending on the chosen model

If any subdivision entry is uncertain, document that the seed data is representative and not complete.

## Data-source documentation

Update `docs/data-sources.md`.

It must honestly explain:

- source concepts,
- what seed data is included,
- data completeness limitations,
- no official ISO endorsement,
- future data expansion plan.

Do not claim official completeness unless you actually implemented complete, properly sourced data.

## Tests

Use xUnit unless the bootstrap already selected something else.

Required test areas:

### Value object tests

- alpha-2 parses uppercase and lowercase,
- alpha-2 rejects null/empty/whitespace,
- alpha-2 rejects wrong length,
- alpha-2 rejects digits/symbols,
- alpha-3 equivalent tests,
- numeric preserves leading zeroes,
- numeric rejects invalid length/chars,
- subdivision parses representative valid codes,
- subdivision rejects invalid formats.

### Registry tests

- lookup `GB` by alpha-2 returns United Kingdom,
- lookup `GBR` by alpha-3 returns same entry,
- lookup `826` by numeric returns same entry,
- lookup `008` preserves Albania numeric code,
- unknown syntactically valid code returns false/result failure, not crash,
- `GetBy...` throws clearly for unknown codes,
- `TryGetBy...` handles invalid input safely.

### JSON tests

- serialises canonical codes,
- deserialises valid codes,
- rejects invalid codes.

### Data integrity tests

- no duplicate alpha-2,
- no duplicate alpha-3,
- no duplicate numeric,
- all seed country codes pass syntax validation,
- all subdivision records refer to known countries,
- subdivision country prefix matches the separate country field if present.

## Samples

Create at least two compiling samples.

### `CountryLookup.Console`

Demonstrate:

- parse alpha-2, alpha-3, numeric,
- lookup mixed user input,
- print canonical country metadata,
- handle invalid/unknown input.

### `CsvImport.Validation`

Demonstrate:

- validate a few in-memory CSV-like records,
- keep row-level errors,
- show canonical country codes for valid rows,
- do not crash on bad input.

Samples must compile as part of the solution.

## README update

Replace the bootstrap-only README with a useful first-pass README.

Include:

- what the package is,
- what it is not,
- installation placeholder,
- quick-start examples,
- alpha-2 / alpha-3 / numeric explanation,
- subdivision-code explanation,
- JSON example,
- CSV import example,
- country dropdown idea,
- data-source transparency,
- known limitations,
- contribution/data-correction guidance.

Keep the README clear and practical.

## Package metadata

Ensure `dotnet pack --configuration Release` works.

Add/verify:

- package ID,
- title,
- description,
- authors,
- licence expression,
- repository URL,
- project URL,
- tags:
  - `countries`
  - `iso3166`
  - `m49`
  - `country-codes`
  - `dotnet`
  - `validation`
- README included in package if practical.
- Source Link if practical.

Do not publish the package.

## CI

Update `.github/workflows/build.yml` if needed so it runs:

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build --output ./artifacts
```

Use bash-compatible commands in GitHub Actions unless specifically targeting Windows.

## Verification commands

Run these before claiming completion:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release --output ./artifacts
```

If any command fails, fix the cause if reasonable. If blocked by missing local SDK/tooling, report exactly what failed.

## Commit behaviour

If Git is available and the work is coherent:

```powershell
git status
git add .
git commit -m "Implement initial ISOCodex.Countries foundation"
```

If committing fails due to Git identity, do not fake it. Report the exact issue.

If a GitHub remote exists, do not push automatically unless Tony has explicitly asked for push behaviour in this session or repo instructions. Prefer to leave the branch locally committed and report the push command.

## Acceptance criteria

The first pass is complete when:

- solution exists,
- core library exists,
- tests exist and pass,
- samples compile,
- pack succeeds,
- README is useful,
- data-source limitations are honest,
- there are no hidden runtime network calls,
- no NuGet publishing occurred.

## Final response requirements

Report:

- what was implemented,
- key public types,
- how to run verification,
- current data coverage,
- known limitations,
- whether tests/build/pack passed,
- whether a commit was created,
- next suggested task.

Do not start a second development pass unless Tony explicitly asks.
