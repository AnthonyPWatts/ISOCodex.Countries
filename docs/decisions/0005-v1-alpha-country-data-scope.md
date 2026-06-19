# 0005: v1 Alpha Country Data Scope

## Status

Accepted.

## Context

The package originally targeted `0.1.0` while country data was representative only. That limited release posture avoided implying complete country-code coverage before a redistribution-safe data source was chosen.

Unicode CLDR 48.2 now provides the package's pinned source for current country and territory code coverage. The package generates 249 current ISO-style alpha-2, alpha-3, numeric, and English display-name entries from CLDR territory mappings and English territory names.

The package still does not redistribute official ISO tables, does not claim ISO endorsement, and does not model full ISO 3166-2 subdivision coverage.

## Decision

Target `1.0.0-alpha` for the next release-candidate package.

The v1 alpha country-data contract is:

- current country and territory entries are derived from Unicode CLDR 48.2;
- deprecated territory aliases are excluded;
- CLDR pseudo-territories, regional groupings, unknown-region placeholders, and user-assigned code elements that are not ISO 3166-1 assigned country entries are excluded;
- subdivision code/name coverage is handled separately by [`0006-v1-alpha-subdivision-data-scope.md`](0006-v1-alpha-subdivision-data-scope.md).

## Consequences

The package can move beyond the earlier `0.1.0` representative-country posture.

The alpha label remains important because consumer feedback may still affect API ergonomics, data-source wording, and future reserved-code or subdivision modelling.

Future CLDR updates must be generated through `eng/update-country-seed-from-cldr.ps1`, reviewed, and covered by data drift and package verification.
