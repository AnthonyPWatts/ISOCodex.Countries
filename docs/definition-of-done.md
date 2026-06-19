# Definition of Done

## Scope

`ISOCodex.Countries` is done for a release candidate when it provides stable, dependency-light value objects, registry lookup, JSON support, documentation, tests, and packaging checks for the data scope being claimed.

## Non-goals

The package does not provide sanctions data, flags, calling codes, currencies, address formatting, geospatial data, localisation, online updates, or full subdivision coverage in the core package.

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
- do not treat `EU` or `ZZ` as current countries without explicit model support,
- throw clear exceptions from `GetBy...` for unknown valid codes,
- return false rather than throwing from `TryGet...` methods.

## Data completeness policy

v0.x may use representative seed data when documentation says so clearly.

v1.0 requires either a complete, redistribution-safe current-country dataset or an explicit decision that the package is intentionally scoped to a representative foundation. If the data-source position is unresolved, publish only as v0.x or delay publication.

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

Representative seed data is not complete ISO 3166 coverage. Subdivision coverage proves the model but is not a global dataset.
