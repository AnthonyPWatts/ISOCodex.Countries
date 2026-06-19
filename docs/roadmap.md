# Roadmap

## Current status

`ISOCodex.Countries` is a `1.0.0-alpha` foundation package with CLDR-derived current country and territory seed data, representative subdivision seed data, strong value objects, registry lookup, JSON support, tests, package smoke testing, and release documentation.

The package is not an official ISO product and does not provide full subdivision coverage.

## v1 alpha release path

The v1 alpha path is appropriate now that a redistribution-safe country and territory source has been selected.

The package should continue to:

- keep CLDR generation reproducible;
- document data-source limitations clearly;
- preserve stable API shape where possible;
- keep package verification green;
- avoid silent alias resolution;
- avoid hidden runtime network calls;
- avoid full subdivision expansion in the core package.

## Data-source maintenance plan

1. Keep `eng/update-country-seed-from-cldr.ps1` pinned to a reviewed CLDR release.
2. Update `docs/data-sources.md` and `CountryDataVersion` when the CLDR source release changes.
3. Review generated JSON and compiled data together.
4. Keep tests for expected count, duplicates, syntax, numeric leading zeroes, selected exclusions, and known edge cases.

## Optional subdivision packs

Full global subdivision coverage should not be added to the core package casually.

Possible future package shape:

- `ISOCodex.Countries.Subdivisions.GB`
- `ISOCodex.Countries.Subdivisions.US`
- `ISOCodex.Countries.Subdivisions.All`

Do not create these until the source and update strategy is proven.

## ASP.NET Core integration

A future `ISOCodex.Countries.AspNetCore` package could provide model binding, validation attributes, minimal API binding helpers, and OpenAPI examples.

Build this only after the core API is stable.

## Addressing and currency integration

Future review areas:

- whether `ISOCodex.Addressing` should depend on `ISOCodex.Countries` after the Countries alpha package has been validated;
- whether a currency package should use country code value objects or registry metadata after the Countries alpha package has been validated;
- whether adapter packages are preferable to direct dependencies once the core package surface is stable.

Avoid circular dependencies.

## Explicit non-goals

Do not add sanctions data, flags, calling codes, currency data, address formatting, localisation, geospatial data, online updates, or speculative analyzers/source generators to the core package.
