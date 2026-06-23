# 0005: v1 Alpha Country Data Scope

## Status

Superseded by the stable `1.0.0` release after the alpha package line was validated.

## Context

The package originally targeted `0.1.0` while country data was representative only. That limited release posture avoided implying complete country-code coverage before a redistribution-safe data source was chosen.

Unicode CLDR 48.2 now provides the package's pinned source for current country and territory code coverage. The package generates 249 current ISO-style alpha-2, alpha-3, numeric, and English display-name entries from CLDR territory mappings and English territory names. It also generates selected-locale country display names, explicit aliases, and special country-code-shaped elements from CLDR metadata.

The package still does not redistribute official ISO tables, does not claim ISO endorsement, and does not model official ISO 3166-2 subdivision categories or hierarchy.

## Decision

The original target was `1.0.0-alpha` for the next release-candidate package.

This target was completed by the alpha package line and then promoted to stable `1.0.0`.

The v1 alpha country-data contract is:

- current country and territory entries are derived from Unicode CLDR 48.2;
- deprecated territory aliases are excluded from canonical country lookup but exposed through explicit alias lookup;
- CLDR pseudo-territories, regional groupings, unknown-region placeholders, and user-assigned code elements that are not ISO 3166-1 assigned country entries are excluded from canonical country lookup but exposed through `CountryCodeElementRegistry` where deliberately modelled;
- selected CLDR locale display names are generated as Unicode/NFC-normalised metadata and are not identifiers;
- subdivision code/name coverage is handled separately by [`0006-v1-alpha-subdivision-data-scope.md`](0006-v1-alpha-subdivision-data-scope.md).

## Consequences

The package can move beyond the earlier `0.1.0` representative-country posture.

The alpha label was important during validation because consumer feedback could still affect API ergonomics, alias behaviour, display-name fallback policy, endonym coverage, data-source wording, and future reserved-code or subdivision modelling.

Future CLDR updates must be generated through `eng/update-country-seed-from-cldr.ps1`, reviewed, and covered by data drift and package verification.
