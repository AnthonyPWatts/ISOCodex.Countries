# Versioning

`ISOCodex.Countries` follows Semantic Versioning for public API changes.

The package currently targets stable `1.0.0`.

The v1 package line is appropriate because country, territory, and regular subdivision data is complete for the selected CLDR-derived v1 scope.

The v1 data-scope decision is tracked in `docs/data-strategy.md`, `docs/decisions/0005-v1-alpha-country-data-scope.md`, and `docs/decisions/0006-v1-alpha-subdivision-data-scope.md`.

Validation issue codes should be stable once published.

Breaking changes include:

- removing public APIs,
- changing parse or normalisation semantics,
- changing validation issue codes,
- removing country records,
- changing canonical mappings,
- changing `UK`/`GB` lookup semantics,
- changing failure reasons for existing tested behaviours.

Usually non-breaking changes include:

- adding metadata,
- adding explicit alias APIs without changing canonical lookup,
- adding subdivisions,
- correcting display names without changing identifiers,
- adding source details.

Data updates must be documented so consumers can understand when metadata changed and why.

Update `CountryDataVersion` when the checked data posture changes.
