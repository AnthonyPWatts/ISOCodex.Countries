# Design Notes

The library should prefer value objects over naked strings for country codes and other stable identifiers.

The core library should target .NET Standard 2.1 unless a later implementation decision records a clear reason to change that compatibility baseline. Build tooling and tests may use newer .NET SDKs where appropriate.

Invalid, unknown, and reserved values must be distinct concepts. Display names are not identifiers and must not be treated as canonical keys.

Runtime library code must not make hidden network calls. Country and subdivision metadata should expose source and version transparency so consumers can reason about the data they are using.
