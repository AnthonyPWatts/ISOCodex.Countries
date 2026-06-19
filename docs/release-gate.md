# Release Gate

Use this checklist before any NuGet release. Publishing itself must be a separate explicit action.

## Version and release notes

- [ ] Version chosen intentionally.
- [ ] Release version is intentionally pre-1.0 while data remains representative.
- [ ] No `1.0.0` release is attempted without completing the data-scope decision.
- [ ] `CHANGELOG.md` updated.
- [ ] README reviewed.
- [ ] Known limitations still accurate.

## Data and documentation

- [ ] `docs/data-sources.md` reviewed.
- [ ] Data version reviewed.
- [ ] Data-source and redistribution assumptions documented.
- [ ] No undocumented data-source TODOs.
- [ ] No official ISO endorsement is implied.
- [ ] `GB`, `UK`, `EU`, and `ZZ` behaviour remains documented and tested.

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

## Final release decision

- [ ] No hidden runtime network calls.
- [ ] No accidental public API drift.
- [ ] No accidental JSON/compiled seed drift.
- [ ] Known limitations documented.
- [ ] `0.1.0` release posture still matches `docs/decisions/0003-v0-1-release-posture.md`.
- [ ] NuGet publishing performed separately and intentionally.
