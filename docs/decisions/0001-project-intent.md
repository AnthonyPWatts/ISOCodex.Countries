# 0001: Project Intent

## Status

Accepted

## Context

`ISOCodex.Addressing` and related ISOCodex package work suggest a broader pattern for small, standards-aware domain packages. Country and jurisdiction metadata is a natural shared foundation for address handling, currency-related workflows, and other business-domain libraries.

## Decision

Create `ISOCodex.Countries` as the shared foundation for country, country-code, subdivision, and jurisdiction data in the ISOCodex family.

## Consequences

Future ISOCodex packages may depend on this package for country and jurisdiction concepts. The package must be transparent about data sources and data versions. It must not present itself as a political authority or an official ISO product.
