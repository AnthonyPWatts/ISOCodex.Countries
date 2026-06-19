# Design Notes

The library should prefer value objects over naked strings for country codes and other stable identifiers.

Invalid, unknown, and reserved values must be distinct concepts. Display names are not identifiers and must not be treated as canonical keys.

Runtime library code must not make hidden network calls. Country and subdivision metadata should expose source and version transparency so consumers can reason about the data they are using.
