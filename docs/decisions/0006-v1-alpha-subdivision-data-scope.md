# 0006: v1 Alpha Subdivision Data Scope

## Status

Accepted.

## Context

The package originally included only representative subdivision examples. Unicode CLDR 48.2 provides redistribution-safe subdivision validity data and English subdivision display names.

CLDR does not provide the ISO 3166-2 subdivision category wording used by ISO. The current package can therefore provide broad subdivision code and name lookup without claiming complete subdivision type classification.

## Decision

Include all regular CLDR 48.2 subdivision identifiers that have English subdivision names.

The v1 alpha subdivision-data contract is:

- 5,027 regular subdivision records generated from CLDR 48.2;
- 200 countries with subdivision records;
- English subdivision display names from `common/subdivisions/en.xml`;
- subdivision code validity from `common/validity/subdivision.xml`;
- subdivision type is `Unknown` unless a deliberately reviewed overlay exists.

## Consequences

Consumers can validate and look up known subdivision codes broadly through `CountrySubdivisionRegistry`.

The package still does not claim official ISO redistribution, official ISO subdivision type names, complete localisation, hierarchy/containment modelling, or complete political authority.

Future subdivision category/type enrichment needs a separate source decision.
