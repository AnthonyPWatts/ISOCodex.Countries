# Roadmap

## Current status

`ISOCodex.Countries` is a v0.x foundation package with representative country and subdivision seed data, strong value objects, registry lookup, JSON support, tests, package smoke testing, and release documentation.

It is not yet a complete current-country dataset.

## v0.x release path

The v0.x path is appropriate while the data-source position remains unresolved.

v0.x should continue to:

- document representative data clearly,
- preserve stable API shape where possible,
- keep package verification green,
- avoid silent alias resolution,
- avoid hidden runtime network calls,
- avoid full subdivision expansion in the core package.

## v1.0 release path

v1.0 requires the deliberate data decision described in [`data-strategy.md`](data-strategy.md):

- either complete, redistribution-safe current-country data,
- or an explicit product decision that the package is intentionally representative.

If complete data is chosen, document the source, version, date checked, redistribution basis, update process, and expected count before adding the data.

## Data-source expansion plan

1. Identify a credible redistribution-safe source path.
2. Update `docs/data-sources.md` before expanding data.
3. Decide whether JSON is the source and compiled C# is generated, or whether C# remains the source and JSON is mirrored reference data.
4. Add tests for expected count, duplicates, syntax, numeric leading zeroes, and known awkward codes.
5. Update `CountryDataVersion`.

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

- whether `ISOCodex.Addressing` should depend on `ISOCodex.Countries`,
- whether a currency package should use country code value objects or registry metadata.

Avoid circular dependencies.

## Explicit non-goals

Do not add sanctions data, flags, calling codes, currency data, address formatting, localisation, geospatial data, online updates, or speculative analyzers/source generators to the core package.
