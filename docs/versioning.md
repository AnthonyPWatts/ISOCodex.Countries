# Versioning

`ISOCodex.Countries` follows Semantic Versioning for public API changes.

The package should remain pre-1.0 while it contains representative data only, unless there is an explicit product decision that representative scope is the v1.0 contract.

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
