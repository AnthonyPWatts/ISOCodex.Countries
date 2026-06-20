# Design Notes

The library should prefer value objects over naked strings for country codes and other stable identifiers. Value objects make the difference between "text that looks like a code" and "a syntactically valid canonical code" explicit in consuming code.

The core library should target .NET Standard 2.1 unless a later implementation decision records a clear reason to change that compatibility baseline. Build tooling and tests may use newer .NET SDKs where appropriate.

Invalid, unknown, and reserved values must be distinct concepts. Display names are not identifiers and must not be treated as canonical keys.

`GB` is the canonical country code for the United Kingdom in this package. `UK` is common in real systems, but silently treating it as `GB` would hide data quality issues and make persistence ambiguous. Alias support is therefore explicit through `CountryAliasRegistry`.

`EU`, `QO`, `XA`, `XB`, `XK`, and `ZZ` are not treated as current countries by the current registry. They remain syntactically valid alpha-2 inputs, but mixed lookup returns `ReservedButNotCountry` so consumers can distinguish known special-purpose values from invalid syntax.

Special code elements are exposed through `CountryCodeElementRegistry`. This is deliberately separate from `CountryRegistry` because a country-code-shaped value is not necessarily a current country.

Display names are metadata, not identifiers. CLDR-derived names may be Unicode, non-Latin, right-to-left, or locale-specific. They are preserved as source strings, NFC-normalised, and never transliterated by the package.

There is intentionally no `ToLocal()` helper. Countries may have multiple local languages, scripts, and endonyms, and some localised names are source-derived display labels rather than authoritative self-names. Endonym APIs should stay explicit and conservative.

`CountryEntryStatus` and `CountryCodeLookupFailureReason` include some future-facing values for reserved, former, exceptional, transitional, user-assigned, ambiguous, and unsupported cases. The current CLDR-derived country and territory seed data uses only `CountryEntryStatus.Current`; consumers must not infer official reservation status from an `Unknown` lookup result.

Runtime library code must not make hidden network calls. Country and subdivision metadata should expose source and version transparency so consumers can reason about the data they are using.

Subdivision code/name coverage is included for the selected CLDR-derived regular subdivision scope. `CountryRegistry.Lookup` reports subdivision-shaped values such as `GB-ENG` as `Unsupported`; `CountrySubdivisionRegistry.Lookup` is the explicit API for subdivision lookup. Official ISO subdivision category/type wording, hierarchy modelling, and richer geopolitical metadata remain separate data-source and maintenance problems rather than core value-object concerns.
