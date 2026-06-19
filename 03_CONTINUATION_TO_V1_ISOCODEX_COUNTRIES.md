# Codex Continuation Brief: ISOCodex.Countries — from current foundation to release-ready package

## Purpose

This brief continues `ISOCodex.Countries` from the current repository state towards the anticipated endpoint: a release-ready, boringly useful, standards-aware .NET package for country and subdivision identifiers.

The audience is Codex Desktop working against Tony's local repository.

This brief intentionally starts from the repository as it exists now, not from the original idealised plan. Inspect the repo before making changes and adapt to the actual implementation.

## Current assumed position

Assume the bootstrap and first-pass foundation are largely complete.

The current public repo appears to contain:

```text
ISOCodex.Countries.sln
src/
  ISOCodex.Countries/
tests/
  ISOCodex.Countries.Tests/
samples/
  CountryLookup.Console/
  CsvImport.Validation/
data/
  countries.seed.json
  subdivisions.seed.json
docs/
  data-sources.md
  design-notes.md
  versioning.md
.github/
  workflows/
    build.yml
```

The implemented core appears to include:

- `CountryAlpha2Code`
- `CountryAlpha3Code`
- `CountryNumericCode`
- `CountrySubdivisionCode`
- `CountryInfo`
- `CountrySubdivisionInfo`
- `CountryEntryStatus`
- `CountrySubdivisionType`
- `CountryCodeValidationIssue`
- `CountryCodeKind`
- `CountryCodeLookupFailureReason`
- `CountryCodeLookupResult`
- `CountryRegistry`
- `CountrySubdivisionRegistry`
- `System.Text.Json` converters for the value-object types
- representative compiled seed data
- representative JSON seed files
- xUnit tests for value objects, registries, JSON converters, and data integrity
- two console-style samples
- package metadata
- GitHub Actions restore/build/test/pack workflow

Treat this as the starting point to verify, not as guaranteed truth.

## Pragmatic changes since the original brief

The original first-pass brief was intentionally foundation-oriented. It was not intended to solve data completeness or official ISO redistribution.

A few pragmatic changes appear to have happened since that brief was written:

1. **Target framework changed from `netstandard2.0` to `netstandard2.1`.**

   Do not casually revert this. Keep `netstandard2.1;net8.0` unless you find a strong consumer-driven reason to restore `netstandard2.0`.

2. **Runtime data is compiled into `CountrySeedData`.**

   The original brief allowed either embedded resources or generated static data. The current static compiled-data approach is acceptable. The next step is to protect it against drift from the JSON seed files, not to rewrite it for its own sake.

3. **JSON converters live in the core package.**

   Keep this for now. Do not split a separate JSON package unless a real dependency or packaging problem appears.

4. **`UK` is currently not silently resolved to `GB`.**

   Preserve this principle. If alias support is added, it must be explicit and named as alias support. Do not make ordinary `GetByAlpha2`, `TryGetByAlpha2`, or `Lookup` silently translate `UK` to `GB`.

5. **The project is probably at the point previously described as “around Epic 5”.**

   Treat Epics 0-4 as mostly complete:
   - repository scaffold,
   - first implementation,
   - initial tests,
   - docs and samples,
   - basic package/CI polish.

   Continue with Epic 5 below.

6. **Old follow-up brief numbering may be inconsistent.**

   If the repo or working folder contains both of these as `02_...` files:

   ```text
   02_PUBLIC_API_REVIEW_ISOCODEX_COUNTRIES.md
   02_DOD_HARDENING_ISOCODEX_COUNTRIES.md
   ```

   fix the sequence before continuing. Suggested sequence:

   ```text
   00_BOOTSTRAP_ISOCODEX_COUNTRIES.md
   01_FIRST_PASS_ISOCODEX_COUNTRIES.md
   02_PUBLIC_API_REVIEW_ISOCODEX_COUNTRIES.md
   03_DOD_HARDENING_ISOCODEX_COUNTRIES.md
   04_DATA_EXPANSION_AND_RELEASE_READINESS_ISOCODEX_COUNTRIES.md
   ```

   If those files are not present in the repo, do not create a pile of duplicate instruction files unless useful. This continuation brief can be used directly.

## Product endpoint

The anticipated endpoint is **not** “a massive geopolitical database”.

The target endpoint is:

> `ISOCodex.Countries` is a tested, documented, packageable, source-transparent .NET library that provides strongly typed country and subdivision identifiers, current-country registry lookups, clear failure semantics, JSON support, and trustworthy release gates.

For an eventual v1.0, the desired bar is:

- stable public API,
- complete or deliberately scoped current-country coverage,
- transparent data-source/version policy,
- strong data integrity tests,
- public API drift protection,
- package smoke testing,
- release checklist,
- CI green,
- no hidden runtime network calls,
- no claims of official ISO endorsement,
- no NuGet publishing unless Tony explicitly asks.

If full current-country data cannot be included safely because of redistribution or licensing uncertainty, do not fake it. Leave the package as a hardened v0.x foundation, document the gap, and provide a precise data-sourcing task for later.

## Non-goals to preserve

Do not add:

- sanctions/compliance data,
- real-time political or diplomatic status updates,
- flags or emoji flags,
- calling codes,
- currency data,
- address formatting,
- full localisation,
- geospatial polygons,
- full global ISO 3166-2 subdivision data in the core package,
- online update services,
- hidden runtime network calls,
- NuGet publishing.

Do not claim official ISO endorsement.

Do not silently resolve common-but-non-canonical codes as canonical countries.

Do not rewrite the project because a cleaner architecture is imaginable. This pass is about continuing and hardening a working foundation.

---

# Epic 5 — Current-state audit and sequence cleanup

## Goal

Establish exactly what is implemented, what is merely documented, and what is still missing.

## Required actions

1. Inspect the repository structure.

2. Run the baseline verification commands:

   ```powershell
   dotnet restore
   dotnet build --configuration Release
   dotnet test --configuration Release
   dotnet pack --configuration Release --output ./artifacts
   ```

3. If any command fails, fix the cause if it is straightforward.

4. If blocked by SDK/tooling/environment, capture the exact command and error output.

5. Create:

   ```text
   docs/current-state-audit.md
   ```

## `docs/current-state-audit.md` should include

Keep this concise. Include:

- commit/branch checked,
- .NET SDK used,
- solution/projects found,
- target frameworks,
- public domain types found,
- registry behaviour found,
- JSON converter behaviour found,
- seed data coverage,
- tests found,
- samples found,
- docs found,
- CI workflow found,
- package metadata found,
- verification command results,
- gaps and risks.

Suggested sections:

```markdown
# Current State Audit

## Checked environment

## Projects and target frameworks

## Public API summary

## Registry and data coverage

## Tests and samples

## Documentation

## Package and CI

## Verification results

## Gaps

## Recommended next actions
```

## Acceptance criteria

Epic 5 is complete when:

- restore/build/test/pack either pass or blockers are documented,
- `docs/current-state-audit.md` exists,
- obsolete/duplicated instruction numbering is cleaned up if those files exist,
- no functional rewrite has been attempted yet.

Commit suggestion:

```powershell
git add .
git commit -m "Audit current ISOCodex.Countries state"
```

If nothing changed except local verification, no commit is required.

---

# Epic 6 — Public API review and low-risk refinement

## Goal

Make sure the public API is pleasant, coherent, domain-first, hard to misuse, and stable enough to harden.

This is not a feature expansion epic.

## Required actions

Create:

```text
docs/public-api-review.md
```

Build a public API inventory. Organise it by concept rather than dumping raw reflection noise.

Suggested sections:

```markdown
# Public API Review

## Value objects

## Registries and lookup results

## Country metadata

## Subdivision metadata

## Validation and failure types

## JSON support

## Current consumer experience

## API concerns found

## Changes implemented

## Changes deferred
```

## Review principles

### 1. Domain types should remain central

Preferred consumer flow:

```csharp
CountryAlpha2Code code = CountryAlpha2Code.Parse("GB");
CountryInfo country = CountryRegistry.GetByAlpha2(code);
```

String overloads are allowed, but they should be convenience APIs. They should not become the conceptual centre of the package.

### 2. Syntax and registry knowledge must remain separate

These are different states:

- invalid syntax,
- syntactically valid but unknown,
- known current country,
- reserved/special/non-country,
- alias/common-but-not-canonical value.

Do not collapse these into vague `false`/`null` behaviour where the public API has richer result types available.

### 3. `GB`, `UK`, `EU`, and `ZZ` must be deliberately handled

Review and document current behaviour for:

- `GB` — canonical United Kingdom alpha-2 code.
- `UK` — commonly encountered, but not silently canonical `GB`.
- `EU` — not a country and must not be silently treated as one.
- `ZZ` — syntactically valid alpha-2 shape, but not a normal current country entry.

If the existing implementation treats `UK`, `EU`, and `ZZ` as `Unknown`, that is acceptable for the current stage, provided it is documented and tested.

Only add richer failure reasons such as `ReservedButNotCountry` if there is explicit data/model support for them. Do not invent a half-supported status.

### 4. Value objects should be boring and predictable

Review:

- default value behaviour,
- `Value` and `ToString()` consistency,
- `Parse` exception messages,
- `TryParse` behaviour,
- `IsValidSyntax`,
- `TryValidate` / structured issue behaviour if implemented,
- equality,
- comparison,
- case handling,
- whitespace handling,
- non-ASCII rejection,
- numeric leading-zero handling,
- nullable annotations.

Do not casually change public struct/class shape unless there is a clear reason. If the value objects are currently `readonly struct`s, keep them unless there is a serious API correctness issue.

### 5. Metadata objects should be immutable from the consumer point of view

Review:

- `CountryInfo`,
- `CountrySubdivisionInfo`,
- collection properties,
- alias/note collections,
- public constructors,
- nullability.

Avoid public mutable collections.

### 6. JSON support should be explicit and tested

If converters require manual registration, keep the README honest.

Do not add automatic global converter magic unless it is simple, dependency-light, and tested.

## Add public API ergonomics tests

Create:

```text
tests/ISOCodex.Countries.Tests/PublicApiErgonomicsTests.cs
```

Include consumer-shaped tests for:

- parse alpha-2 then registry lookup,
- `TryGet` from untrusted input,
- mixed lookup success,
- mixed lookup failure,
- `UK` not resolving silently to `GB`,
- JSON DTO round trip,
- country dropdown/listing pattern,
- subdivision lookup pattern.

These tests are usage-shape tests. They are not the formal API snapshot.

## Low-risk refinements allowed

Allowed if clearly beneficial:

- improve exception messages,
- add missing XML docs,
- add missing tests,
- rename an obviously confusing type before publication,
- add a missing `TryParse`/`IsValidSyntax` that was intended,
- make alias behaviour explicit,
- improve result/failure naming,
- align README/sample code with the real API.

Avoid:

- full subdivision registry redesign,
- package splitting,
- data-source rewrite,
- large dependencies,
- source generators,
- analyzers,
- exhaustive country data expansion in this epic.

## Acceptance criteria

Epic 6 is complete when:

- `docs/public-api-review.md` exists,
- the current public API has been inventoried,
- low-risk API refinements are implemented or explicitly deferred,
- public API ergonomics tests exist,
- README and samples still match actual API,
- restore/build/test/pack pass or blockers are documented.

Commit suggestion:

```powershell
git add .
git commit -m "Review and refine public API shape"
```

---

# Epic 7 — Definition-of-Done hardening

## Goal

Turn the package from “works” into “trustworthy”.

The definition of done should protect public promises, data integrity, packaging behaviour, and documentation accuracy.

## Required documents

Create:

```text
docs/dod-gap-analysis.md
docs/definition-of-done.md
docs/release-gate.md
```

## `docs/dod-gap-analysis.md`

Summarise:

- already satisfied DoD items,
- missing DoD items,
- deferred items,
- blocked items,
- data-source concerns,
- recommended order of attack.

Do not make this a long essay. Codex and Tony should be able to scan it.

## `docs/definition-of-done.md`

This should become the durable package-quality checklist.

Cover:

- scope,
- non-goals,
- value-object correctness,
- registry behaviour,
- data completeness policy,
- data-source transparency,
- JSON behaviour,
- test strategy,
- package quality,
- public API stability,
- documentation,
- versioning,
- release process,
- known limitations.

Include a clear v1.0 target and a clear v0.x acceptable state.

Suggested language:

> v1.0 requires either a complete, redistribution-safe current-country dataset or an explicit decision that the package is intentionally scoped to a representative foundation. If the data-source position is unresolved, publish only as v0.x or delay publication.

## `docs/release-gate.md`

Create a practical checklist Tony can use before any NuGet release.

Include:

- version chosen,
- changelog updated,
- README reviewed,
- data-source docs reviewed,
- package metadata reviewed,
- local restore/build/test/pack passed,
- package smoke test passed,
- CI green,
- no accidental runtime network calls,
- no undocumented data-source TODOs,
- known limitations documented,
- NuGet publishing performed separately and intentionally.

## Strengthen tests

Add or expand tests for:

### Value objects

- lowercase canonicalisation,
- whitespace rejection,
- wrong length,
- digits/symbols rejection,
- non-ASCII rejection,
- numeric leading zeroes,
- numeric `FromInt32` if present,
- `default` value behaviour if applicable,
- equality and comparison,
- `TryValidate` issue codes if present.

### Registry behaviour

- `GB`, `GBR`, `826` resolve to same entry,
- `008` resolves to Albania and preserves leading zero,
- `UK` does not resolve as `GB`,
- `EU` does not resolve as a country,
- `ZZ` is syntactically valid but unknown,
- invalid strings do not throw from `TryGet`/`Lookup`,
- `GetBy...` throws clear exceptions for unknown valid codes,
- subdivision registry known and unknown cases.

### JSON behaviour

- each value object serialises as canonical string,
- each value object deserialises valid values,
- invalid values throw `JsonException`,
- DTO round trip works,
- nullable properties behave intentionally,
- converters preserve numeric leading zeroes.

### Data integrity

- no duplicate alpha-2,
- no duplicate alpha-3,
- no duplicate numeric,
- no duplicate subdivision code,
- all compiled seed countries pass syntax validation,
- all compiled seed subdivisions refer to known countries,
- subdivision country prefix matches `CountryCode`,
- JSON seed files and compiled seed data are either intentionally identical or differences are documented,
- data files remain valid JSON.

If JSON seed files are not consumed at runtime, add tests or tooling to prevent silent drift between `data/*.seed.json` and `CountrySeedData.cs`.

## Package smoke testing

Add either a test project or a script.

Preferred lightweight script:

```text
eng/smoke-test-package.ps1
```

The script should:

```powershell
dotnet pack --configuration Release --output ./artifacts
dotnet new console -n PackageSmokeConsumer -o ./artifacts/PackageSmokeConsumer --force
dotnet add ./artifacts/PackageSmokeConsumer/PackageSmokeConsumer.csproj package ISOCodex.Countries --source ./artifacts
# Replace Program.cs with a tiny lookup using CountryAlpha2Code and CountryRegistry.
dotnet run --project ./artifacts/PackageSmokeConsumer/PackageSmokeConsumer.csproj --configuration Release
```

The smoke test must prove:

- the `.nupkg` installs,
- the package exposes usable APIs,
- runtime lookup works from the package,
- no loose data-file dependency breaks runtime usage,
- package metadata is sufficient.

Wire this into CI if practical. If not practical, document why in `docs/dod-gap-analysis.md`.

## Public API drift protection

Add public API protection if practical.

Acceptable options:

- `PublicApiGenerator`,
- `PublicApiAnalyzer`,
- a small reflection-based snapshot test.

A simple solution is fine. The goal is to catch accidental public API drift before NuGet publication.

Suggested structure:

```text
tests/
  ISOCodex.Countries.ApiApprovalTests/
    PublicApiTests.cs
    approved-public-api.txt
```

or, if keeping one test project:

```text
tests/ISOCodex.Countries.Tests/PublicApiSnapshotTests.cs
tests/ISOCodex.Countries.Tests/approved-public-api.txt
```

Document how to update the snapshot intentionally.

## CI hardening

Ensure CI still runs:

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build --output ./artifacts
```

If a smoke-test script exists, add it after pack if it is reliable on GitHub Actions.

Do not add NuGet publishing.

## Acceptance criteria

Epic 7 is complete when:

- `docs/dod-gap-analysis.md` exists,
- `docs/definition-of-done.md` exists,
- `docs/release-gate.md` exists,
- value-object tests cover normal and awkward cases,
- registry tests cover canonical, unknown, alias-like, and invalid cases,
- data integrity tests protect compiled and JSON seed data,
- JSON tests cover round-tripping and invalid input,
- package smoke testing exists or is explicitly deferred with reason,
- public API protection exists or is explicitly deferred with reason,
- CI remains restore/build/test/pack,
- restore/build/test/pack pass or blockers are documented.

Commit suggestion:

```powershell
git add .
git commit -m "Harden ISOCodex.Countries definition of done"
```

---

# Epic 8 — Data-source strategy and current-country expansion

## Goal

Move from “representative seed data” towards the desired release data posture without violating licensing or pretending to be an official source.

This is the highest-risk epic because it touches redistribution and correctness. Proceed carefully.

## Product decision to make first

Before expanding data, decide the release target:

### Option A — Hardened v0.x foundation

Use the existing representative dataset.

Appropriate if:

- source licensing remains uncertain,
- Tony wants to prove package shape before data work,
- the package is not being published as “complete current-country coverage”.

Requirements:

- documentation must say representative only,
- README must not imply completeness,
- package version should remain pre-1.0,
- data-source roadmap should be precise.

### Option B — v1.0-ready current-country package

Expand to complete current-country coverage.

Appropriate if:

- a redistribution-safe source path is identified,
- data provenance can be documented,
- tests can protect all identifiers,
- the package can honestly describe its coverage.

Requirements:

- complete current country/territory dataset for alpha-2, alpha-3, and numeric codes,
- clear source/version/date checked,
- no copied data without rights,
- data integrity tests,
- data update process,
- changelog/data-version notes.

Do not proceed with Option B unless the data-source position is credible.

## Add a data version concept

Add a simple public or internal concept for data versioning.

Possible public shape:

```csharp
public static class CountryDataVersion
{
    public static string Identifier { get; }
    public static DateOnly? CheckedOn { get; }
    public static string Description { get; }
}
```

If `DateOnly` creates target-framework pain, use a string or `DateTimeOffset`.

Do not over-engineer. The point is source/version transparency, not a full data governance framework.

At minimum, documentation should clearly state:

- data version,
- date checked,
- source concepts,
- known gaps.

## Protect data-source drift

The current repo appears to have both JSON seed files and compiled `CountrySeedData`.

That is acceptable only if drift is controlled.

Choose one of these approaches:

### Approach 1 — JSON is source, generated C# is runtime

Add a small generator under:

```text
tools/ISOCodex.Countries.DataGen/
```

or a script under:

```text
eng/
```

It should:

- read `data/countries.seed.json`,
- read `data/subdivisions.seed.json`,
- validate schema/integrity,
- generate `CountrySeedData.cs`,
- include a generated-file header,
- be deterministic.

Add a test that fails if generated output differs from checked-in output.

### Approach 2 — C# is source, JSON is documentation/reference

Document that clearly and add a test that checks the JSON files mirror the compiled data.

This is simpler, but less elegant.

Preferred direction: Approach 1 if it can be done without thrashing. Otherwise Approach 2 is fine.

## Expand country data if safe

If proceeding with complete current-country coverage:

1. Identify source and redistribution basis.
2. Add or update `docs/data-sources.md` before adding large data.
3. Expand `data/countries.seed.json`.
4. Regenerate or update compiled data.
5. Add tests that verify:
   - count is expected for the chosen source/version,
   - no duplicate alpha-2,
   - no duplicate alpha-3,
   - no duplicate numeric,
   - all numeric codes are three digits,
   - all alpha codes are uppercase canonical,
   - all records have non-empty English short names,
   - known awkward values behave intentionally.

Do not include former, reserved, exceptional, or user-assigned codes in the “current countries” list unless the API model explicitly supports them.

## Subdivision data policy

Do not attempt full ISO 3166-2 global subdivision coverage in the core package as part of this epic.

Recommended policy:

- keep representative subdivisions in core,
- document that they prove the model rather than complete coverage,
- design for later country-specific or optional subdivision data packs.

Possible future package pattern:

```text
ISOCodex.Countries.Subdivisions.GB
ISOCodex.Countries.Subdivisions.US
ISOCodex.Countries.Subdivisions.All
```

Do not create those packages yet unless Tony explicitly asks.

## Aliases and special territories

Do not casually add alias resolution.

If alias support is added, use explicit naming such as:

```csharp
CountryRegistry.TryResolveAlias("UK", out CountryInfo? country)
```

or:

```csharp
CountryAliasRegistry.TryResolve("UK", out CountryAliasResolution? result)
```

Document that alias resolution is not the same as canonical code lookup.

For `EU`, `UK`, `ZZ`, exceptional reservations, user-assigned ranges, and former codes, prefer a future explicit model rather than mixing them into current-country lookup.

## Acceptance criteria

Epic 8 is complete when one of these is true:

### v0.x foundation outcome

- data-source uncertainty is honestly documented,
- current representative dataset is protected against drift,
- data version policy exists,
- data expansion task is precise and deferred,
- README clearly says not data-complete.

### v1.0-ready current-country outcome

- complete current-country data exists from a credible source path,
- source/version/date checked are documented,
- compiled and JSON data are synchronised,
- data integrity tests protect completeness,
- README accurately describes coverage,
- no redistribution uncertainty is hidden.

Commit suggestion:

```powershell
git add .
git commit -m "Clarify and protect country data sources"
```

If complete current-country data is added:

```powershell
git add .
git commit -m "Expand current country registry data"
```

---

# Epic 9 — Documentation, samples, and release readiness

## Goal

Make the package easy to understand, consume, verify, and release intentionally.

## README hardening

Update `README.md` to match actual behaviour.

It must include:

- what the package is,
- what it is not,
- installation status,
- quick start,
- alpha-2 explanation,
- alpha-3 explanation,
- numeric/M49-style code explanation,
- subdivision-code explanation,
- `GB`/`UK` caveat,
- `EU` caveat if tested/documented,
- JSON converter example,
- CSV import example,
- country dropdown example,
- persistence guidance,
- data-source transparency,
- known limitations,
- contribution/data-correction guidance.

Add a short persistence guidance section:

```markdown
## Persistence Guidance

For most applications, store canonical alpha-2 codes such as `GB`.

Store alpha-3 or numeric codes only when an integration requires them.

Do not store display names as identifiers.

If preserving what a user originally typed matters, store the original input separately from the canonical code.
```

## Docs hardening

Update:

```text
docs/data-sources.md
docs/design-notes.md
docs/versioning.md
```

### `docs/data-sources.md`

Must include:

- source concepts used,
- actual current data coverage,
- data version,
- date checked if known,
- no official ISO endorsement,
- redistribution/licensing caveats,
- update process,
- known gaps,
- how to propose data corrections.

### `docs/design-notes.md`

Must include:

- why value objects exist,
- why display names are not identifiers,
- why invalid/unknown/reserved are distinct,
- why `UK` is not silently `GB`,
- why no hidden network calls exist,
- why full subdivision coverage is deferred.

### `docs/versioning.md`

Must include:

- SemVer policy,
- public API changes,
- validation issue-code stability,
- data-version policy,
- what counts as breaking:
  - removing country records,
  - changing canonical mappings,
  - changing validation issue codes,
  - changing parse semantics,
  - removing public APIs,
- what is usually non-breaking:
  - adding metadata,
  - adding aliases via explicit APIs,
  - adding subdivisions,
  - correcting display names without changing identifiers.

## Sample hardening

Ensure samples compile and still demonstrate useful consumer flows.

### `samples/CountryLookup.Console`

Should demonstrate:

- parse alpha-2,
- parse alpha-3,
- parse numeric,
- mixed lookup,
- unknown valid code,
- invalid input,
- `UK` caveat.

### `samples/CsvImport.Validation`

Should demonstrate:

- row-level validation,
- canonical output,
- invalid syntax error,
- unknown valid code error,
- alias-like input such as `UK`,
- no crash for bad input.

If useful, add comments explaining why each case matters.

## Package metadata

Review:

- package ID,
- title,
- description,
- authors,
- licence,
- repository URL,
- project URL,
- tags,
- README in package,
- Source Link,
- deterministic build,
- symbols if currently configured.

Do not publish.

## Changelog

Update `CHANGELOG.md`.

Use Keep-a-Changelog style.

For pre-release work, keep entries under `Unreleased`.

Mention:

- first foundation implementation,
- API review/hardening,
- data-source limitations,
- tests and package smoke testing,
- any data expansion.

## Verification

Run:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release --output ./artifacts
```

If implemented:

```powershell
./eng/smoke-test-package.ps1
```

Run samples if practical:

```powershell
dotnet run --project samples/CountryLookup.Console --configuration Release
dotnet run --project samples/CsvImport.Validation --configuration Release
```

## Acceptance criteria

Epic 9 is complete when:

- README accurately represents actual package behaviour,
- docs are consistent,
- samples compile and demonstrate real flows,
- changelog is updated,
- package metadata is reviewed,
- release gate checklist exists,
- verification commands pass or blockers are documented,
- no NuGet publishing occurred.

Commit suggestion:

```powershell
git add .
git commit -m "Prepare ISOCodex.Countries for release review"
```

---

# Epic 10 — Future integration planning

## Goal

Record useful next steps without bloating the core package before it is stable.

Do not implement these unless Tony explicitly asks.

## Candidate future work

### ASP.NET Core integration

Possible package:

```text
ISOCodex.Countries.AspNetCore
```

Potential features:

- model binding for value objects,
- validation attributes,
- minimal API binding helpers,
- OpenAPI/schema examples.

Only do this after the core API is stable.

### Optional subdivision packs

Possible packages:

```text
ISOCodex.Countries.Subdivisions.GB
ISOCodex.Countries.Subdivisions.US
ISOCodex.Countries.Subdivisions.All
```

Only do this after data-source strategy is proven.

### Addressing/Currency integration

Later, review whether:

- `ISOCodex.Addressing` should depend on `ISOCodex.Countries`,
- `ISOCodex.Currency` should use `CountryAlpha2Code` or country registry metadata where helpful.

Do not create circular dependencies.

### Analyzers/source generators

Only consider if there is a clear consumer pain point.

Examples might include:

- detecting display-name persistence,
- validating hard-coded country codes,
- generating country dropdowns.

Do not add analyzers/source generators as speculative polish.

## Required output

Create or update:

```text
docs/roadmap.md
```

Include:

- current package status,
- v0.x/v1.0 release path,
- data-source expansion plan,
- subdivision-pack idea,
- ASP.NET Core integration idea,
- Addressing/Currency integration idea,
- explicit non-goals.

Commit suggestion:

```powershell
git add .
git commit -m "Document ISOCodex.Countries roadmap"
```

---

# Final verification before handing back to Tony

Before claiming completion for this continuation brief, run:

```powershell
git status
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release --output ./artifacts
```

If available and implemented:

```powershell
./eng/smoke-test-package.ps1
dotnet run --project samples/CountryLookup.Console --configuration Release
dotnet run --project samples/CsvImport.Validation --configuration Release
```

Do not claim a command passed unless it actually passed.

If something fails, report:

- exact command,
- exact failure summary,
- whether it was fixed,
- what remains blocked.

Do not publish to NuGet.

Do not push unless Tony explicitly asked for push behaviour.

## Suggested final Codex response format

When finished, report:

```markdown
## Summary

- ...

## Implemented

- ...

## Documentation added/updated

- ...

## Tests added/updated

- ...

## Data-source position

- ...

## Verification

| Command | Result |
|---|---|
| `dotnet restore` | Pass/Fail/Blocked |
| `dotnet build --configuration Release` | Pass/Fail/Blocked |
| `dotnet test --configuration Release` | Pass/Fail/Blocked |
| `dotnet pack --configuration Release --output ./artifacts` | Pass/Fail/Blocked |
| `./eng/smoke-test-package.ps1` | Pass/Fail/Blocked/Not implemented |

## Commits

- ...

## Known limitations

- ...

## Recommended next task

- ...
```

## Overall acceptance criteria for this continuation

The continuation work is complete when the repository has:

- current-state audit,
- public API review,
- API ergonomics tests,
- definition-of-done documentation,
- release gate checklist,
- hardened tests for awkward cases,
- data integrity drift protection,
- JSON DTO/invalid/nullable tests,
- package smoke testing or a documented deferment,
- public API snapshot protection or a documented deferment,
- accurate README/docs/samples,
- data-source/version policy,
- clear v0.x/v1.0 decision,
- green restore/build/test/pack or exact documented blockers,
- no hidden runtime network calls,
- no NuGet publishing.
