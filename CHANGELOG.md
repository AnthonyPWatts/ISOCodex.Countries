# Changelog

## 1.0.0-alpha.1 - 2026-06-20

Documentation update for the v1 alpha package line.

### Changed

- Clarified the difference between alpha-2 syntax validation, current-country registry lookup, and special code-element lookup.
- Updated package release notes for the `1.0.0-alpha.1` documentation package.

## 1.0.0-alpha - 2026-06-20

Alpha release candidate for the v1 foundation contract.

### Added

- Added CLDR-derived current country and territory seed data for 249 alpha-2, alpha-3, numeric, and English display-name entries.
- Added CLDR-derived country display-name seed data for selected locales, including non-Latin and right-to-left examples.
- Added `CountryNameRegistry`, display-name lookup results, explicit fallback semantics, and conservative endonym APIs.
- Added explicit `CountryAliasRegistry` lookup using reviewed common aliases and CLDR deprecated territory aliases.
- Added public `CountryCodeElementRegistry` metadata for known special non-country alpha-2-shaped values.
- Added rich `CountrySubdivisionRegistry.Lookup` result semantics.
- Added CLDR-derived regular subdivision seed data for 5,027 subdivision entries across 200 countries.
- Added `eng/update-country-seed-from-cldr.ps1` to regenerate country seed data from pinned Unicode CLDR source files.
- Added `eng/update-subdivision-seed-from-cldr.ps1` to regenerate subdivision seed data from pinned Unicode CLDR source files.
- Added third-party notices for Unicode CLDR data.
- Added data tests for expected country count, CLDR source disclosure, and selected territory edge cases.

### Changed

- Changed package version posture from `0.1.0` to `1.0.0-alpha`.
- Updated data-source, versioning, release-gate, and roadmap documentation for the CLDR-derived country-data scope.

### Release notes

- Country and territory data is derived from Unicode CLDR 48.2, not copied from official ISO tables.
- Country display names are CLDR-derived labels, not official government names. Unicode source text is preserved and NFC-normalised; no transliteration is performed.
- Alias lookup is opt-in. Canonical country-code lookup does not resolve aliases or display names.
- Subdivision code and English display-name data is derived from Unicode CLDR 48.2, not copied from official ISO tables.
- Subdivision type metadata is intentionally sparse and remains `Unknown` unless a specific reviewed overlay exists.
- The package is not an official ISO product and does not claim ISO endorsement.

## 0.1.0 - 2026-06-19

Initial pre-1.0 foundation release.

### Added

- Added strongly typed country and subdivision code value objects.
- Added representative country and subdivision registries.
- Added structured validation issue codes and mixed country-code lookup results.
- Added `System.Text.Json` converters for country-code value objects.
- Added data-version exposure through `CountryDataVersion`.
- Added data integrity, JSON converter, data drift, public API, and package smoke tests.
- Added package metadata, CI restore/build/test/pack, package artifact upload, and release-gate documentation.

### Documentation

- Documented data-source limitations, redistribution assumptions, and the representative seed-data posture.
- Documented that JSON converters require manual registration.
- Documented that the runtime package uses checked-in compiled seed data and does not make hidden network calls.

### Release notes

- This is a pre-1.0 release candidate for the foundation API.
- Seed data is representative only; it is not complete ISO 3166 coverage.
- The package is not an official ISO product and does not claim ISO endorsement.
