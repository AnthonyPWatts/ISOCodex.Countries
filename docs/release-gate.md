# Release Gate

Use this checklist before any NuGet release. Publishing itself must be a separate explicit action.

## Version and release notes

- [ ] Version chosen intentionally.
- [ ] Release version is intentionally `1.0.0-alpha.1`.
- [ ] v1 alpha data scope matches `docs/decisions/0005-v1-alpha-country-data-scope.md`.
- [ ] `CHANGELOG.md` updated.
- [ ] README reviewed.
- [ ] Known limitations still accurate.

## Data and documentation

- [ ] `docs/data-sources.md` reviewed.
- [ ] Data version reviewed.
- [ ] CLDR source release and generation policy reviewed.
- [ ] Country names seed generation verified.
- [ ] Unicode and non-Latin script examples tested.
- [ ] Right-to-left display-name example tested.
- [ ] Alias registry behaviour tested.
- [ ] Special code-element registry tested.
- [ ] Subdivision lookup result tested.
- [ ] Country lookup unsupported-shape behaviour tested.
- [ ] Data-source and redistribution assumptions documented.
- [ ] Country and subdivision generated counts reviewed.
- [ ] Public API snapshot intentionally updated.
- [ ] No undocumented data-source TODOs.
- [ ] No official ISO endorsement is implied.
- [ ] `GB`, `UK`, `EU`, `QO`, `XA`, `XB`, `XK`, and `ZZ` behaviour remains documented and tested.

## Package metadata

- [ ] Package ID reviewed.
- [ ] Title and description reviewed.
- [ ] Authors reviewed.
- [ ] Licence reviewed.
- [ ] Repository URL and project URL reviewed.
- [ ] Tags reviewed.
- [ ] README included in package.
- [ ] Package contains expected `lib/netstandard2.1` and `lib/net8.0` assets.
- [ ] Package contains XML documentation files when XML docs are enabled.
- [ ] Package contains `THIRD-PARTY-NOTICES.md`.
- [ ] Package does not contain accidental loose runtime `data/*.json` dependencies.
- [ ] Source Link and deterministic build settings still present.

## Local verification

Run:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release --output ./artifacts
./eng/smoke-test-package.ps1
dotnet run --project samples/CountryLookup.Console --configuration Release
dotnet run --project samples/CsvImport.Validation --configuration Release
```

All commands must pass or the blocker must be documented before release.

## CI

- [ ] CI is green on the release commit.
- [ ] CI restores, builds, tests, packs, and runs package smoke testing.
- [ ] CI uploads package artifacts for the release-candidate commit.

## NuGet publishing workflow

Publishing is handled by `.github/workflows/publish-nuget.yml`.

Before publishing:

- [ ] The GitHub `release` environment exists.
- [ ] NuGet trusted publishing or `NuGet/login` is configured for this repository and environment.
- [ ] The NuGet trusted-publishing policy allows user `AnthonyPWatts`, repository `AnthonyPWatts/ISOCodex.Countries`, workflow `.github/workflows/publish-nuget.yml`, and environment `release`.
- [ ] A matching release tag exists, such as `v1.0.0-alpha.1`, or the workflow is run manually with the same tag value.
- [ ] The package version in `ISOCodex.Countries.csproj` matches the release tag without the leading `v`.

## Final release decision

- [ ] No hidden runtime network calls.
- [ ] No accidental public API drift.
- [ ] No accidental JSON/compiled seed drift.
- [ ] Known limitations documented.
- [ ] v1 alpha release posture still matches `docs/decisions/0005-v1-alpha-country-data-scope.md`.
- [ ] NuGet publishing performed separately and intentionally.
