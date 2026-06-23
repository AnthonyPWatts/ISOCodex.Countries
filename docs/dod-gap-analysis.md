# Definition-of-Done Gap Analysis

## Already satisfied

- Strongly typed value objects exist for alpha-2, alpha-3, numeric, and subdivision codes.
- Runtime lookup uses compiled seed data with no hidden network calls.
- Registry APIs distinguish invalid syntax from unknown syntactically valid input.
- JSON converters exist and are tested.
- Restore, build, test, and pack pass locally.
- Package metadata, README packaging, Source Link, deterministic build settings, and CI exist.

## Added in this hardening pass

- Current-state audit.
- Public API review.
- Public API ergonomics tests.
- Public API snapshot protection.
- JSON seed versus compiled data drift tests.
- Package smoke-test script.
- Release gate checklist.
- Explicit v1 alpha data-source posture.
- Data version metadata.

## Missing for a v1.0 data-complete package

- Complete current-country dataset from a redistribution-safe source path.
- Documented source version and source date for complete data.
- A repeatable data update process if JSON becomes the source of generated compiled data.
- Explicit modelling for reserved, former, exceptional, and user-assigned codes if those behaviours are promised.
- A deliberate subdivision data-pack strategy if consumers need broad ISO 3166-2 coverage.

## Deferred

- Complete localisation.
- Full global subdivision coverage.
- Alias resolution.
- ASP.NET Core integration.
- Analyzers or source generators.
- NuGet publishing.

## Data-source concerns

The package now contains CLDR-derived country, territory, and regular subdivision data for the selected v1 alpha scope.

Do not copy large official datasets into the repository until redistribution rights are clear.

## Recommended order of attack

1. Keep v1 alpha package hardening green.
2. Review consumer feedback on the CLDR-derived country and territory scope.
3. Resolve data-source and redistribution position.
4. Expand country data only after documentation and tests define the source/version/date checked.
5. Consider generated compiled data only after source data ownership is settled.
