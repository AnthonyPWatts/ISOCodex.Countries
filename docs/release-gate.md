# Release Gate

Use this checklist before any NuGet release. Publishing itself must be a separate explicit action.

## Version and release notes

- [ ] Version chosen intentionally.
- [ ] Pre-1.0 version retained if data remains representative.
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

## Final release decision

- [ ] No hidden runtime network calls.
- [ ] No accidental public API drift.
- [ ] No accidental JSON/compiled seed drift.
- [ ] Known limitations documented.
- [ ] NuGet publishing performed separately and intentionally.
