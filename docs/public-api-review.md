# Public API Review

This review covers the first implementation pass of `ISOCodex.Countries`. The goal is to check whether the public API is pleasant, coherent, stable enough to harden later, and aligned with the broader ISOCodex style.

## Value Objects

### `CountryAlpha2Code`

Represents a canonical two-letter country code.

Public surface:

- `string Value`
- `static CountryAlpha2Code Parse(string value)`
- `static bool TryParse(string? value, out CountryAlpha2Code code)`
- `static bool IsValidSyntax(string? value)`
- `static CountryCodeValidationIssue? TryValidate(string? value, out string normalized)`
- `bool Equals(CountryAlpha2Code other)`
- `override bool Equals(object? obj)`
- `override int GetHashCode()`
- `int CompareTo(CountryAlpha2Code other)`
- `int CompareTo(object? obj)`
- `override string ToString()`
- equality, inequality, less-than, and greater-than operators

Construction is intentionally factory-based. The only constructor is private, so invalid values cannot be created through public constructors.

### `CountryAlpha3Code`

Represents a canonical three-letter country code.

Public surface:

- `string Value`
- `static CountryAlpha3Code Parse(string value)`
- `static bool TryParse(string? value, out CountryAlpha3Code code)`
- `static bool IsValidSyntax(string? value)`
- `static CountryCodeValidationIssue? TryValidate(string? value, out string normalized)`
- `bool Equals(CountryAlpha3Code other)`
- `override bool Equals(object? obj)`
- `override int GetHashCode()`
- `int CompareTo(CountryAlpha3Code other)`
- `int CompareTo(object? obj)`
- `override string ToString()`
- equality and inequality operators

The name is verbose but clear and consistent with `CountryAlpha2Code`.

### `CountryNumericCode`

Represents a canonical three-digit numeric country code, preserving leading zeroes.

Public surface:

- `string Value`
- `static CountryNumericCode Parse(string value)`
- `static CountryNumericCode FromInt32(int value)`
- `static bool TryParse(string? value, out CountryNumericCode code)`
- `static bool IsValidSyntax(string? value)`
- `static CountryCodeValidationIssue? TryValidate(string? value, out string normalized)`
- `bool Equals(CountryNumericCode other)`
- `override bool Equals(object? obj)`
- `override int GetHashCode()`
- `int CompareTo(CountryNumericCode other)`
- `int CompareTo(object? obj)`
- `override string ToString()`
- equality and inequality operators

`CountryNumericCode` is preferable to `CountryM49Code` for now because the package is using ISO-style numeric country codes aligned with UN M49 concepts, not claiming to model the whole UN M49 standard.

### `CountrySubdivisionCode`

Represents an ISO 3166-2-style subdivision code.

Public surface:

- `CountryAlpha2Code CountryCode`
- `string SubdivisionPart`
- `string Value`
- `static CountrySubdivisionCode Parse(string value)`
- `static bool TryParse(string? value, out CountrySubdivisionCode code)`
- `static bool IsValidSyntax(string? value)`
- `static CountryCodeValidationIssue? TryValidate(string? value, out CountryAlpha2Code countryCode, out string subdivisionPart)`
- `bool Equals(CountrySubdivisionCode other)`
- `override bool Equals(object? obj)`
- `override int GetHashCode()`
- `int CompareTo(CountrySubdivisionCode other)`
- `int CompareTo(object? obj)`
- `override string ToString()`
- equality and inequality operators

The type makes syntax validation separate from registry knowledge, which is the right split.

## Registry And Lookups

### `CountryRegistry`

Static registry for country lookup.

Public surface:

- `IReadOnlyList<CountryInfo> All`
- `CountryInfo GetByAlpha2(CountryAlpha2Code code)`
- `CountryInfo GetByAlpha3(CountryAlpha3Code code)`
- `CountryInfo GetByNumeric(CountryNumericCode code)`
- `bool TryGetByAlpha2(CountryAlpha2Code code, out CountryInfo? country)`
- `bool TryGetByAlpha3(CountryAlpha3Code code, out CountryInfo? country)`
- `bool TryGetByNumeric(CountryNumericCode code, out CountryInfo? country)`
- `bool TryGetByAlpha2(string? value, out CountryInfo? country)`
- `bool TryGetByAlpha3(string? value, out CountryInfo? country)`
- `bool TryGetByNumeric(string? value, out CountryInfo? country)`
- `CountryCodeLookupResult Lookup(string? value)`

`CountryRegistry` remains the best name among the reviewed options. `Countries` is too broad, `CountryLookup` sounds operation-only, and `CountryCatalog` or `CountryDirectory` do not clearly improve discoverability.

`GetBy...` methods throw `KeyNotFoundException` for unknown syntactically valid codes. `TryGetBy...` methods return `false` for invalid or unknown input. Mixed lookup returns a structured `CountryCodeLookupResult`.

### `CountrySubdivisionRegistry`

Static registry for representative subdivision lookup.

Public surface:

- `IReadOnlyList<CountrySubdivisionInfo> All`
- `CountrySubdivisionInfo GetByCode(CountrySubdivisionCode code)`
- `bool TryGetByCode(CountrySubdivisionCode code, out CountrySubdivisionInfo? subdivision)`
- `bool TryGetByCode(string? value, out CountrySubdivisionInfo? subdivision)`

The name is clear and maps to the package terminology.

## Country Metadata

### `CountryInfo`

Immutable consumer-facing country metadata.

Public constructor:

- `CountryInfo(CountryAlpha2Code alpha2, CountryAlpha3Code alpha3, CountryNumericCode numeric, string englishShortName, string? englishOfficialName, CountryEntryStatus status, IEnumerable<string>? commonAliases = null, IEnumerable<string>? notes = null)`

Public properties:

- `CountryAlpha2Code Alpha2`
- `CountryAlpha3Code Alpha3`
- `CountryNumericCode Numeric`
- `string EnglishShortName`
- `string? EnglishOfficialName`
- `CountryEntryStatus Status`
- `IReadOnlyList<string> CommonAliases`
- `IReadOnlyList<string> Notes`
- `override string ToString()`

`CountryInfo` remains preferable to plain `Country`, because it is clearly a metadata record rather than a real-world geopolitical entity.

### `CountryEntryStatus`

Enum values:

- `Current`
- `Former`
- `Reserved`
- `ExceptionallyReserved`
- `TransitionallyReserved`
- `UserAssigned`
- `Unknown`

Only `Current` is used in the first seed data. The remaining values are retained because they describe expected registry states and are useful for future data expansion.

## Subdivision Metadata

### `CountrySubdivisionInfo`

Immutable consumer-facing subdivision metadata.

Public constructor:

- `CountrySubdivisionInfo(CountrySubdivisionCode code, CountryAlpha2Code countryCode, string englishName, string? localName, CountrySubdivisionType type)`

Public properties:

- `CountrySubdivisionCode Code`
- `CountryAlpha2Code CountryCode`
- `string EnglishName`
- `string? LocalName`
- `CountrySubdivisionType Type`
- `override string ToString()`

### `CountrySubdivisionType`

Enum values:

- `Unknown`
- `State`
- `Province`
- `Territory`
- `Region`
- `County`
- `District`
- `Department`
- `Municipality`
- `Parish`
- `CouncilArea`
- `Nation`
- `Other`

The enum is intentionally broad but not exhaustive.

## Validation And Failure Types

### `CountryCodeValidationIssue`

Structured syntax validation issue.

Public constructor:

- `CountryCodeValidationIssue(string code, string message, string? input)`

Public properties:

- `string Code`
- `string Message`
- `string? Input`

Stable issue-code examples include `country.alpha2.empty`, `country.alpha2.invalid_length`, `country.alpha2.invalid_characters`, `country.numeric.invalid_characters`, and `country.subdivision.invalid_format`.

### `CountryCodeLookupResult`

Structured result for mixed user/API input.

Public properties:

- `bool Success`
- `CountryInfo? Country`
- `CountryCodeKind? DetectedKind`
- `CountryCodeLookupFailureReason? FailureReason`
- `string? NormalizedInput`

The constructor and factories are not public, which keeps result creation controlled by the registry.

### `CountryCodeKind`

Enum values:

- `Alpha2`
- `Alpha3`
- `Numeric`

### `CountryCodeLookupFailureReason`

Enum values:

- `Empty`
- `InvalidSyntax`
- `Ambiguous`
- `Unknown`
- `ReservedButNotCountry`
- `Unsupported`

`Ambiguous` and `Unsupported` are currently future-facing. `ReservedButNotCountry` is now used for `UK`, `EU`, and `ZZ` in mixed lookup.

## JSON Support

Public converter types:

- `CountryAlpha2CodeJsonConverter`
- `CountryAlpha3CodeJsonConverter`
- `CountryNumericCodeJsonConverter`
- `CountrySubdivisionCodeJsonConverter`

Each converter serialises to a canonical string and rejects invalid strings with `JsonException`.

Public helper:

- `CountryJsonSerializerOptions.CreateDefault()`
- `CountryJsonSerializerOptions.AddConverters(JsonSerializerOptions options)`

The helper keeps converter registration discoverable without adding a new package or dependency.

## Samples And Consumer Experience

The README and samples cover:

- parse and lookup by value object,
- mixed lookup from boundary input,
- CSV-style row validation,
- JSON converter registration,
- dropdown/listing generation from `CountryRegistry.All`.

The examples now use `CountryJsonSerializerOptions` for converter registration and explain that `UK`, `EU`, and `ZZ` are not silently treated as countries.

## API Concerns

- `CountryRegistry.Lookup("UK")`, `Lookup("EU")`, and `Lookup("ZZ")` previously returned generic `Unknown`. That blurred reserved/special/non-country cases with ordinary unknown values.
- JSON support was correct but slightly clumsy because every consumer had to discover and register four converter classes manually.
- `CountryEntryStatus` and `CountryCodeLookupFailureReason` include future-facing values. This is acceptable for this early foundation package, but the DoD hardening pass should document enum compatibility expectations carefully.
- Public metadata constructors are useful for tests and future generated/static data, but they mean consumers can create metadata records outside the registry. That is acceptable for now because records are immutable and validation occurs in the value objects.

## Recommended Changes

- Keep `CountryRegistry`, `CountryInfo`, `CountryNumericCode`, and subdivision type names.
- Keep value object construction factory-based.
- Keep string overloads as boundary conveniences, not the centre of the API.
- Make reserved/special alpha-2 lookup outcomes explicit for `UK`, `EU`, and `ZZ`.
- Add a JSON converter registration helper.
- Add consumer-style API ergonomics tests.

## Changes Implemented

- Added reserved/special handling in mixed lookup for `UK`, `EU`, and `ZZ`.
- Added `CountryJsonSerializerOptions` with `CreateDefault()` and `AddConverters(...)`.
- Updated README JSON examples and `UK`/`EU`/`ZZ` behaviour documentation.
- Added `PublicApiErgonomicsTests`.
- Added XML documentation to `CountryCodeLookupFailureReason` values.

## Changes Deferred

- No alias-resolution API was added. If needed later, it should be explicit, for example `TryResolveAlias`, and backed by clear data policy.
- No generated static known-code list was added. A large list such as `CountryAlpha2Code.GB` through every current country would bloat the early API.
- No public API snapshot system was added. That belongs in the DoD hardening pass.
- No full subdivision redesign was attempted.
- No localisation, flags, calling codes, currency, sanctions, or address-formatting features were added.
