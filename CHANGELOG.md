# Changelog

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
