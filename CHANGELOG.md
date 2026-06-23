# Changelog

## 1.0.0 - 2026-06-23

Initial stable release of `ISOCodex.Countries`.

### Includes

- Strongly typed alpha-2, alpha-3, numeric country-code, and subdivision-code value objects.
- Country and subdivision registries backed by a representative compiled seed dataset.
- Mixed country-code lookup with explicit reserved/special handling for `UK`, `EU`, and `ZZ`.
- Structured validation issues suitable for import pipelines and boundary input handling.
- `System.Text.Json` converters and `CountryJsonSerializerOptions` helper APIs.
- Sample projects for lookup and CSV-style validation workflows.

### Limitations

- The seed dataset is intentionally representative, not a complete ISO 3166, ISO 3166-2, UN M49, or CLDR dataset.
