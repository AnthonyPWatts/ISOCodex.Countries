# Design Notes

The library should prefer value objects over naked strings for country codes and other stable identifiers. Value objects make the difference between "text that looks like a code" and "a syntactically valid canonical code" explicit in consuming code.

The core library should target .NET Standard 2.1 unless a later implementation decision records a clear reason to change that compatibility baseline. Build tooling and tests may use newer .NET SDKs where appropriate.

Invalid, unknown, and reserved values must be distinct concepts. Display names are not identifiers and must not be treated as canonical keys.

`GB` is the canonical country code for the United Kingdom in this package. `UK` is common in real systems, but silently treating it as `GB` would hide data quality issues and make persistence ambiguous. If alias support is added later, it should be explicit.

`EU` and `ZZ` are not treated as current countries by the current registry. They remain unknown syntactically valid alpha-2 inputs until a deliberate reserved-code model exists.

`CountryEntryStatus` and `CountryCodeLookupFailureReason` include some future-facing values for reserved, former, exceptional, transitional, user-assigned, ambiguous, and unsupported cases. The current representative seed data uses only `CountryEntryStatus.Current`; consumers must not infer official reservation status from an `Unknown` lookup result.

Runtime library code must not make hidden network calls. Country and subdivision metadata should expose source and version transparency so consumers can reason about the data they are using.

Full subdivision coverage is deferred because it is a data-source, licensing, package-size, and maintenance problem rather than a core value-object problem.
