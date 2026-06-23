# 0004: Defer Addressing And Currency Integration

## Status

Accepted.

## Context

`ISOCodex.Countries` is intended to become a shared foundation for future ISOCodex package work, including possible `ISOCodex.Addressing` and currency-related integration.

The Countries package should first pass its own release gate and settle its alpha package surface before other packages depend on it.

## Decision

Do not add `ISOCodex.Addressing` or `ISOCodex.Currency` package references, adapter packages, or cross-repository integration work until `ISOCodex.Countries` has produced a release candidate.

## Consequences

The core package remains dependency-light.

Integration design can happen later against a stable package surface.

Future adapter packages should be considered only after circular-dependency risks and package ownership boundaries are clear.
