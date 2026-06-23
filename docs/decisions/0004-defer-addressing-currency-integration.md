# 0004: Defer Addressing And Currency Integration

## Status

Superseded by the stable `1.0.0` release. The original decision preserved package independence before the Countries surface had been validated.

## Context

`ISOCodex.Countries` is intended to become a shared foundation for future ISOCodex package work, including possible `ISOCodex.Addressing` and currency-related integration.

The Countries package needed to pass its own release gate and settle its package surface before other packages could depend on it. That condition was satisfied by the stable `1.0.0` release.

## Decision

The original decision was to avoid `ISOCodex.Addressing` or `ISOCodex.Currency` package references, adapter packages, or cross-repository integration work until `ISOCodex.Countries` had produced a release candidate.

That release-candidate blocker is now closed. Future integration work should still be deliberate, package-boundary aware, and reviewed for circular dependencies.

## Consequences

The core package remains dependency-light.

Integration design can happen later against a stable package surface.

Future adapter packages should be considered only after circular-dependency risks and package ownership boundaries are clear.
