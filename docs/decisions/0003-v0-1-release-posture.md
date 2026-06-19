# 0003: v0.1 Release Posture

## Status

Accepted.

## Context

`ISOCodex.Countries` currently has useful value objects, registry lookup, JSON support, package metadata, tests, and release verification. Its checked-in country and subdivision data is still a small hand-curated representative seed set.

That data posture is suitable for a pre-1.0 foundation release, but it must not be confused with complete current-country coverage or an official ISO dataset.

## Decision

Release-candidate work targets `0.1.0`, not `1.0.0`.

No `1.0.0` release should happen until the project either has complete, redistribution-safe current-country coverage for the claimed package scope, or an explicit accepted decision that representative scope is the v1 contract.

## Consequences

The package can produce a useful pre-1.0 release candidate without blocking on complete data sourcing.

Data completeness, source selection, and subdivision expansion can continue after `0.1.0` without undermining the current package surface.

Release-facing documentation must continue to make the representative data limit visible.
