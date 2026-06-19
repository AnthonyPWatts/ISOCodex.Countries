# Codex Public API Review Brief: ISOCodex.Countries

## Purpose

This is the **public API review pass** for `ISOCodex.Countries`.

Run this after the first implementation pass from:

```text
01_FIRST_PASS_ISOCODEX_COUNTRIES.md
```

Run this before the DoD hardening pass.

The goal is to review whether the public API shape is pleasant, coherent, stable, and aligned with the broader ISOCodex style **before** extensive tests and API snapshot protection lock it down.

This task is not about adding lots of new features. It is about making sure the package exposes the right concepts in the right way.

## Core question

Does the package give consumers an API that feels:

- obvious,
- boring,
- strongly typed,
- hard to misuse,
- easy to discover,
- consistent with ISOCodex.Addressing and ISOCodex.Currency,
- stable enough to harden with tests and eventually publish?

If not, fix the API shape now, while the package is still young.

## Important context

`ISOCodex.Countries` is intended to be a foundation package for other ISOCodex libraries.

It should model country and subdivision identifiers as domain concepts, not loose strings.

The API should support typical use cases:

- parse a country code from user/API/input data,
- validate syntax without lookup,
- look up known current countries,
- convert alpha-2 / alpha-3 / numeric code forms,
- distinguish invalid / unknown / reserved / alias values,
- serialise value objects cleanly,
- build country dropdowns,
- validate imports,
- support future packages such as Addressing, Currency, BankIdentifiers, and BusinessIdentifiers.

## Non-goals for this pass

Do not:

- add exhaustive country/subdivision data,
- perform DoD hardening,
- add a full public API snapshot system,
- publish to NuGet,
- create a localisation system,
- add flags/emoji/calling codes/sanctions/currency/address formatting,
- rewrite the whole package unnecessarily,
- add large dependencies just to make the API look fancy.

This is an API review and refinement pass.

## Phase 1: Inspect current public API

First inspect the solution created by the first pass.

Build a public API inventory.

Create:

```text
docs/public-api-review.md
```

Include:

- all public types,
- all public constructors,
- all public methods,
- all public properties,
- all public enums and enum values,
- all public exceptions/result types,
- JSON converter public surface,
- sample usage from README/samples.

Do not generate an enormous unreadable dump. Organise it by concept.

Suggested sections:

- Value objects
- Registry/lookups
- Country metadata
- Subdivision metadata
- Validation/failure types
- JSON support
- Samples/consumer experience
- API concerns
- Recommended changes
- Changes implemented
- Changes deferred

## Phase 2: Compare against intended API principles

Review the current API against these principles.

### 1. Domain types first

The API should encourage this:

```csharp
CountryAlpha2Code code = CountryAlpha2Code.Parse("GB");
CountryInfo country = CountryRegistry.GetByAlpha2(code);
```

Not this:

```csharp
string code = "GB";
var country = CountryRegistry.GetByAlpha2(code);
```

String overloads are useful, but the value objects should be the centre of the public API.

### 2. Clear distinction between syntax and registry knowledge

The API must make these different:

- invalid syntax,
- syntactically valid but unknown,
- known current country,
- reserved/special/non-country,
- alias/common-but-not-canonical.

Do not blur these into a single boolean unless the method name is clearly syntax-only or lookup-only.

### 3. Names should explain intent

Review names such as:

- `CountryRegistry`
- `CountryInfo`
- `CountryAlpha2Code`
- `CountryAlpha3Code`
- `CountryNumericCode`
- `CountrySubdivisionCode`
- `CountryCodeLookupResult`
- `CountryEntryStatus`
- `CountryCodeKind`
- `CountryCodeLookupFailureReason`
- `CountryCodeValidationIssue`

Ask:

- Would a consumer understand the type without reading much documentation?
- Is the name too verbose?
- Is the name too vague?
- Does it match the existing ISOCodex naming style?
- Would this still feel right in v1.0?

### 4. Parse/TryParse semantics must be idiomatic

For value objects:

- `Parse` should throw for invalid syntax.
- `TryParse` should return false for invalid syntax.
- `IsValidSyntax` should match `TryParse`.
- `ToString()` should return canonical representation.
- Case normalisation should be predictable.
- Null/empty behaviour should be documented and tested.

### 5. Lookup semantics must be idiomatic

For registry lookups:

- `GetBy...` should throw clear exceptions for unknown known-good syntax.
- `TryGetBy...` should not throw for normal bad input.
- mixed lookup should have a structured result.
- alias resolution, if any, should be explicit.

### 6. Avoid accidental public mutability

Review all public models.

Prefer:

- immutable records or classes,
- get-only properties,
- read-only collections,
- no public setters unless required for serialisation and carefully justified,
- no mutable global registry.

### 7. JSON support should be discoverable

Review whether consumers can easily use JSON converters.

Questions:

- Are converters named predictably?
- Are they registered manually or automatically?
- Is converter registration documented?
- Do converters serialise to canonical strings?
- Do converters reject invalid input clearly?

### 8. Dependency footprint should be minimal

The core package should be dependency-light.

Review whether any dependencies are unnecessary.

### 9. Public exceptions/results should be useful

Review whether the package exposes:

- clear exceptions for parse/lookup failures,
- structured validation issues,
- stable failure reason enums,
- useful result objects.

Avoid scattering ad-hoc strings through the API.

### 10. API should work nicely in consumer code

Create or update small compile-only examples to assess ergonomics.

Examples:

```csharp
var gb = CountryAlpha2Code.Parse("GB");
var country = CountryRegistry.GetByAlpha2(gb);
Console.WriteLine(country.EnglishShortName);
```

```csharp
if (CountryRegistry.TryGetByAlpha2(input, out var country))
{
    Console.WriteLine(country.Alpha3);
}
```

```csharp
CountryCodeLookupResult result = CountryRegistry.Lookup(input);

if (!result.Success)
{
    Console.WriteLine(result.FailureReason);
}
```

```csharp
var dto = JsonSerializer.Deserialize<CustomerDto>(json, options);
```

If these examples feel awkward, improve the API.

## Phase 3: Review names and possible alternatives

Specifically evaluate these API naming decisions.

### Registry entry point

Current likely name:

```csharp
CountryRegistry
```

Review against alternatives:

```csharp
Countries
CountryCatalog
CountryDirectory
CountryLookup
```

Default preference: keep `CountryRegistry` if it is already clear and coherent. Change only if there is a strong reason.

Decision criteria:

- discoverability,
- consistency with existing ISOCodex packages,
- clarity in consuming code,
- future extensibility.

### Metadata model

Current likely name:

```csharp
CountryInfo
```

Review against alternatives:

```csharp
Country
CountryMetadata
CountryRecord
CountryDefinition
```

Default preference: `CountryInfo` or `CountryMetadata`.

Avoid plain `Country` if it creates ambiguity between a real-world country and a data record.

### Numeric code

Current likely name:

```csharp
CountryNumericCode
```

Review against alternatives:

```csharp
CountryM49Code
CountryNumericCountryCode
```

Default preference: `CountryNumericCode`, unless the implementation explicitly treats this as UN M49 and documentation consistently uses M49.

Important: if using `M49` in the type name, be precise and document it. Do not overclaim if the data model is only “ISO-style numeric country code aligned with UN M49”.

### Subdivision names

Review whether:

```csharp
CountrySubdivisionCode
CountrySubdivisionInfo
CountrySubdivisionType
```

feel right.

Default preference: keep them, because they map clearly to ISO 3166-2 terminology.

### Lookup result names

Review:

```csharp
CountryCodeLookupResult
CountryCodeLookupFailureReason
CountryCodeKind
```

These are verbose but clear.

Only shorten if the resulting names remain obvious.

## Phase 4: Review constructors and factories

For each value object, decide the public construction model.

Preferred:

```csharp
CountryAlpha2Code.Parse("GB")
CountryAlpha2Code.TryParse(input, out var code)
```

Avoid exposing public constructors that allow invalid values.

If a constructor exists, it must validate.

For advanced/internal creation, prefer private/internal constructors plus factory methods.

Review whether static known codes should exist.

Possible examples:

```csharp
CountryAlpha2Code.GB
CountryAlpha2Code.US
```

Default: avoid a large generated static property list in v0.1. It can bloat the API. Registry lookup is enough.

## Phase 5: Review result/failure modelling

Review whether the current result/failure model is expressive enough.

Required distinctions:

- empty input,
- invalid syntax,
- unknown,
- reserved/special/non-country,
- alias/non-canonical,
- unsupported.

Do not add distinctions unless the implementation can use them meaningfully.

If the enum contains values that cannot currently occur, either:

- document them as future-facing, or
- remove them until needed.

Avoid fake completeness.

## Phase 6: Review `GB`, `UK`, `EU`, and `ZZ` API behaviour

The public API must make these cases clear.

### `GB`

Canonical United Kingdom alpha-2 code.

### `UK`

Commonly encountered, but not the canonical package country alpha-2 code.

Required:

- `CountryAlpha2Code.Parse("UK")` may succeed syntactically because it is two ASCII letters.
- `CountryRegistry.TryGetByAlpha2("UK", out _)` should not silently return GB unless the method is explicitly alias-aware.
- If alias resolution exists, it must be named clearly.

Examples:

```csharp
CountryRegistry.TryGetByAlpha2("UK", out _) // false
CountryRegistry.TryResolveAlias("UK", out var country) // possible, explicit
```

or:

```csharp
CountryRegistry.Lookup("UK").FailureReason == CountryCodeLookupFailureReason.ReservedButNotCountry
```

Choose the behaviour that best fits the implementation and document it.

### `EU`

Should not be silently treated as a country.

### `ZZ`

Should be syntactically valid as alpha-2 but unknown/reserved/user-assigned depending on documented policy.

## Phase 7: Review documentation examples against API

Check:

- README examples compile,
- sample projects compile,
- docs do not show API that does not exist,
- docs explain the actual API shape,
- naming in docs is consistent,
- examples use value objects, not only strings.

If examples feel clumsy, improve the API rather than merely rewriting docs.

## Phase 8: Implement obvious API refinements

After the review, implement API refinements that are:

- clearly beneficial,
- low risk,
- coherent with the intended design,
- unlikely to be reversed later.

Examples of acceptable changes:

- rename a confusing public type,
- remove accidental public constructors,
- add missing `TryParse`,
- add missing `IsValidSyntax`,
- make models immutable,
- improve result/failure naming,
- make alias resolution explicit,
- adjust JSON converter names,
- improve exception names/messages,
- align sample code with the intended API.

Examples of changes to defer:

- full subdivision registry redesign,
- full data-source rewrite,
- major package split,
- large dependency additions,
- source-generator/analyser work,
- exhaustive static known-code list.

Record deferred decisions in:

```text
docs/public-api-review.md
```

## Phase 9: Add API ergonomics tests

Add lightweight tests that exercise public API usage from a consumer point of view.

Suggested file:

```text
tests/ISOCodex.Countries.Tests/PublicApiErgonomicsTests.cs
```

Test examples:

- typical parse and lookup flow,
- typical `TryGet` flow,
- mixed lookup failure flow,
- JSON DTO flow,
- country dropdown/listing flow if available.

These tests are not formal API snapshots. They are usage-shape tests.

The later DoD hardening pass may add proper public API snapshot protection.

## Phase 10: Verify package still works

Run:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release --output ./artifacts
```

If any command fails, fix the issue if reasonable.

If blocked by local SDK/tooling, report exact command output and blocker.

## Phase 11: Commit behaviour

If Git is available and the work is coherent:

```powershell
git status
git add .
git commit -m "Review and refine public API shape"
```

If committing fails due to Git identity, do not fake it. Report the exact issue.

Do not push unless Tony explicitly asked for push behaviour.

## Acceptance criteria

This task is complete when:

- `docs/public-api-review.md` exists,
- public API inventory and review notes are documented,
- obvious low-risk API refinements have been made,
- deferred API decisions are recorded,
- README/samples match the actual API,
- API ergonomics tests exist,
- restore/build/test/pack pass or blockers are clearly documented,
- no NuGet publishing occurred,
- a coherent Git commit is created if possible.

## Final response requirements

When finished, report:

- public API shape reviewed,
- key issues found,
- changes implemented,
- changes deferred,
- API ergonomics tests added,
- verification command results,
- commit status,
- recommended next task.

Recommended next task after this pass:

```text
03_DOD_HARDENING_ISOCODEX_COUNTRIES.md
```

If the existing DoD hardening file is still named:

```text
02_DOD_HARDENING_ISOCODEX_COUNTRIES.md
```

recommend renaming it to:

```text
03_DOD_HARDENING_ISOCODEX_COUNTRIES.md
```

so the instruction sequence remains clear.
