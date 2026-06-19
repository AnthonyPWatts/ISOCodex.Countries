# Public API Review

## Value objects

The core value objects are:

- `CountryAlpha2Code`
- `CountryAlpha3Code`
- `CountryNumericCode`
- `CountrySubdivisionCode`

They are `readonly struct` types with canonical string values, `Parse`, `TryParse`, `IsValidSyntax`, structured `TryValidate`, equality, comparison, `<`/`>` operators, and `ToString()` support.

Alpha values canonicalise to uppercase ASCII. Numeric values preserve three-character form, including leading zeroes such as `008`.

## Registries and lookup results

`CountryRegistry` provides strongly typed `GetBy...` and `TryGetBy...` methods, string convenience `TryGetBy...` methods, and mixed `Lookup`.

`CountryCodeLookupResult` preserves:

- whether lookup succeeded,
- the resolved `CountryInfo`,
- detected code kind,
- failure reason,
- normalised input when syntax was detected.

This keeps invalid syntax, syntactically valid unknown values, and successful current-country lookups separate.

## Country metadata

`CountryInfo` exposes immutable consumer-facing properties:

- `Alpha2`
- `Alpha3`
- `Numeric`
- `EnglishShortName`
- `EnglishOfficialName`
- `Status`
- `CommonAliases`
- `Notes`

The alias list is metadata only. It is not used for silent canonical lookup.

## Subdivision metadata

`CountrySubdivisionInfo` exposes:

- `Code`
- `CountryCode`
- `EnglishName`
- `LocalName`
- `Type`

`CountrySubdivisionRegistry` supports lookup by `CountrySubdivisionCode` or string input.

## Validation and failure types

`CountryCodeValidationIssue` exposes stable machine-readable issue codes, human-readable messages, and the failed input.

Current entry statuses and lookup failure reasons include future-facing enum values such as `Reserved`, `Former`, `UserAssigned`, `Ambiguous`, `ReservedButNotCountry`, and `Unsupported`. The current registry does not yet contain separate reserved-code or former-code data. Seed countries therefore use `CountryEntryStatus.Current`, and `UK`, `EU`, and `ZZ` are treated as syntactically valid but unknown alpha-2 input at this stage.

## JSON support

The package includes `System.Text.Json` converters for the value-object types. They require manual registration on `JsonSerializerOptions`.

Converters serialise canonical strings, deserialise valid strings, preserve numeric leading zeroes, and throw `JsonException` for invalid strings or non-string JSON tokens. Default value objects serialise as empty strings and should be treated as uninitialised values rather than meaningful canonical codes.

## Current consumer experience

The intended flow remains domain-first:

```csharp
CountryAlpha2Code code = CountryAlpha2Code.Parse("GB");
CountryInfo country = CountryRegistry.GetByAlpha2(code);
```

String overloads are useful for boundary input, but strongly typed values remain the centre of the API.

## API concerns found

- Package version defaulted to `1.0.0` before hardening, despite representative data. It was first corrected to `0.1.0`, then moved to `1.0.0-alpha` after CLDR-derived country and territory data was added.
- JSON and compiled seed data had one note-string drift. This has been corrected and covered by tests.
- The public API previously had no snapshot protection.
- Public API behaviour around `EU` and `ZZ` was implied by tests but not explicitly documented in API review material.

## Changes implemented

- Added `CountryDataVersion`.
- Added consumer-shaped public API ergonomics tests.
- Added public API snapshot protection using `approved-public-api.txt`.
- Strengthened public API snapshot coverage so public operators are reviewed.
- Added drift tests to keep JSON seed files aligned with compiled seed data.
- Added additional registry, JSON, and value-object edge-case tests.
- Updated documentation and samples to match the current API and v1 alpha data posture.

## Changes deferred

- No alias resolution API has been added.
- No reserved-code registry has been added.
- No full subdivision data has been added.
- No full subdivision data has been added.
- No package split has been attempted.
