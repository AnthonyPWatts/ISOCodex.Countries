# Definition of Done

## Scope

`ISOCodex.Countries` is done for a release candidate when it provides stable, dependency-light value objects, registry lookup, JSON support, documentation, tests, and packaging checks for the data scope being claimed.

## Non-goals

The package does not provide sanctions data, flags, calling codes, currencies, address formatting, geospatial data, complete localisation, online updates, official ISO subdivision categories, or subdivision hierarchy modelling in the core package.

## Value-object correctness

Done means value objects:

- canonicalise valid input predictably,
- reject whitespace, wrong length, symbols, and non-ASCII input,
- preserve numeric leading zeroes,
- expose stable validation issue codes,
- behave predictably for equality, comparison, `ToString()`, and `default`.

## Registry behaviour

Done means registry lookups:

- resolve known alpha-2, alpha-3, and numeric codes to the same country entry,
- keep invalid syntax separate from unknown valid code shapes,
- do not silently resolve `UK` to `GB`,
- keep alias lookup explicit and opt-in,
- keep display-name lookup separate from canonical country-code lookup,
- do not treat `EU` or `ZZ` as current countries without explicit model support,
- report subdivision-shaped country lookup input as `Unsupported`,
- throw clear exceptions from `GetBy...` for unknown valid codes,
- return false rather than throwing from `TryGet...` methods.

## Data completeness policy

The v1 alpha package line requires complete current country, territory, selected display-name, explicit alias, special-code-element, and regular subdivision code/name coverage for the selected CLDR-derived scope. The v1 alpha data-scope decisions are recorded in `docs/decisions/0005-v1-alpha-country-data-scope.md` and `docs/decisions/0006-v1-alpha-subdivision-data-scope.md`.

## Data-source transparency

Done means documentation states:

- data version,
- date checked,
- source concepts,
- current coverage,
- known gaps,
- no official ISO endorsement,
- how corrections should be proposed.

## JSON behaviour

Done means JSON converters:

- serialise value objects as canonical strings,
- deserialise valid values,
- reject invalid values with `JsonException`,
- preserve numeric leading zeroes,
- have DTO round-trip tests,
- document manual converter registration.

## Test strategy

Done means tests cover:

- value-object syntax and canonicalisation,
- registry success, unknown, alias-like, and invalid cases,
- display-name fallback, Unicode, NFC, right-to-left, and conservative endonym cases,
- alias lookup, ambiguity, and canonical lookup separation,
- special code-element registry behaviour,
- subdivision lookup result semantics,
- JSON serialisation and deserialisation,
- seed data integrity,
- JSON seed and compiled seed drift,
- public API ergonomics,
- public API snapshot drift.

## Package quality

Done means:

- package metadata is accurate,
- README is included in the package,
- Source Link and deterministic builds remain configured,
- `dotnet pack` succeeds,
- a local package smoke test proves package contents, installation, and runtime lookup from the `.nupkg`.

## Public API stability

Public API changes must be intentional. Update `tests/ISOCodex.Countries.Tests/approved-public-api.txt` only when the public API change is reviewed and accepted.

## Documentation

Done means README, data-source docs, design notes, versioning docs, changelog, and release gate docs agree with the actual implementation.

## Versioning

Use SemVer. Breaking changes include removing public APIs, changing parse semantics, changing validation issue codes, removing country records, or changing canonical mappings.

## Release process

Before any NuGet release, run the checklist in `docs/release-gate.md`. NuGet publishing is separate and intentional; it is never part of normal verification.

## Known limitations

Country, territory, selected display-name, alias, special-code-element, and regular subdivision seed data is derived from Unicode CLDR 48.2 for the selected v1 alpha scope. Endonym coverage and subdivision type metadata are intentionally sparse.
