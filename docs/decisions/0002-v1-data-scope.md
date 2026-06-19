# 0002: v1.0 data scope

## Status

Proposed

## Context

`ISOCodex.Countries` currently has a representative seed dataset. This is acceptable for v0.x when documented clearly, but v1.0 needs an explicit data-scope decision.

Consumers may reasonably expect a package named `Countries` to provide broad current-country coverage. Publishing a `1.0.0` package with representative data only would therefore be a product-positioning decision, not a default release step.

## Options

1. Complete current-country coverage for v1.0.
2. Representative foundation as the v1.0 contract.
3. Remain v0.x until complete data sourcing is solved.

## Decision

Pending maintainer decision.

## Current recommendation

Remain v0.x until complete current-country data sourcing is solved.

## Consequences

If option 1 is chosen, the project needs a redistribution-safe source path, pinned source snapshot, generated data pipeline, completeness tests, and clear attribution/licence documentation before `1.0.0`.

If option 2 is chosen, the package can reach `1.0.0` sooner, but the README, package description, release notes, and data-source docs must make representative scope impossible to miss.

If option 3 is chosen, `0.x` releases can deliver useful API and package quality while avoiding a misleading completeness claim.

## Deferred work

Do not add full subdivision coverage, reserved-code registries, Addressing integration, or Currency integration as part of this decision. Country-level data scope should be resolved first.
