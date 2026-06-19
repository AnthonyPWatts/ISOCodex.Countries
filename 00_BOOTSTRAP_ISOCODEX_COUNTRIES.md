# Codex Bootstrap Brief: Create ISOCodex.Countries From Scratch

## Purpose

This is the **setup-only** task for a brand-new project: `ISOCodex.Countries`.

The goal of this pass is to create a clean local Git repository and the initial project-management/documentation scaffold so the next Codex task can perform the first real development pass.

Do **not** implement the actual library in this task. This task is about creating the safe starting point.

## Assumed starting point

You are running inside Codex Desktop against an empty local folder chosen by Tony.

The folder may contain only these instruction files:

- `00_BOOTSTRAP_ISOCODEX_COUNTRIES.md`
- `01_FIRST_PASS_ISOCODEX_COUNTRIES.md`

If the folder already contains unrelated files, stop and explain what you found.

## Repository identity

Create a new repository/project with the following identity:

- Repository name: `ISOCodex.Countries`
- Solution name: `ISOCodex.Countries.sln`
- Root namespace: `ISOCodex.Countries`
- Intended NuGet package: `ISOCodex.Countries`
- Intended GitHub owner: `AnthonyPWatts`
- Intended GitHub repo: `https://github.com/AnthonyPWatts/ISOCodex.Countries`

Use UK English in human-facing documentation.

## Important rules

- Do not create application/library code in this bootstrap pass.
- Do not attempt NuGet publishing.
- Do not create large generated data files.
- Do not over-engineer.
- Do not delete the instruction files.
- Do not rewrite `01_FIRST_PASS_ISOCODEX_COUNTRIES.md` unless fixing obvious filename/path references.
- Create a clean checkpoint commit at the end if Git is available.
- If GitHub remote creation fails, leave the local repo in a good state and provide exact manual follow-up instructions.

## Required bootstrap deliverables

Create:

```text
README.md
LICENSE
CHANGELOG.md
.gitignore
.editorconfig
global.json
Directory.Build.props
Directory.Packages.props
AGENTS.md
docs/
  decisions/
    0001-project-intent.md
  data-sources.md
  design-notes.md
  versioning.md
.github/
  workflows/
    build.yml
src/
tests/
samples/
tools/
data/
  README.md
.codex/
  README.md
```

Do **not** create the actual `.sln` or `.csproj` files in this bootstrap pass unless it is clearly needed for repository/tooling sanity. The first development pass will create the solution and projects.

## File content requirements

### `README.md`

Create a concise project stub explaining:

- `ISOCodex.Countries` is a planned .NET library for strongly typed country, country-code, subdivision, and jurisdiction metadata.
- It is intended as a standards-aware domain foundation package.
- It is not an official ISO product and is not a geopolitical authority.
- Initial implementation work is described in `01_FIRST_PASS_ISOCODEX_COUNTRIES.md`.

Include a “Current status” section saying:

> Bootstrap scaffold only. No package has been implemented yet.

### `LICENSE`

Use MIT licence unless there is clear local evidence that Tony’s ISOCodex repositories use another licence.

Set copyright holder as:

```text
Copyright (c) Anthony Watts
```

### `CHANGELOG.md`

Create a Keep-a-Changelog-style stub:

```markdown
# Changelog

## Unreleased

- Created initial repository scaffold.
```

### `.gitignore`

Use a sensible Visual Studio / .NET `.gitignore`.

Include common entries for:

- `bin/`
- `obj/`
- `.vs/`
- `.vscode/` only if appropriate; do not block useful workspace settings if you intentionally add them later
- `TestResults/`
- `artifacts/`
- `*.user`
- `*.suo`
- `*.nupkg`
- `*.snupkg`

### `.editorconfig`

Use a C#/.NET-friendly editorconfig:

- UTF-8
- final newline
- trim trailing whitespace
- 4 spaces for C#
- nullable-friendly conventions
- prefer explicit accessibility where useful
- consistent namespace style

Do not make style rules so strict that early development becomes noisy.

### `global.json`

Pin to a current installed .NET SDK if one is already installed.

First inspect:

```powershell
dotnet --list-sdks
```

Prefer the latest installed .NET 8 SDK.

If no SDK is installed or `dotnet` is unavailable, create no misleading `global.json`; instead document the missing prerequisite in the final response.

### `Directory.Build.props`

Create shared build settings suitable for future library projects:

- `LangVersion` latest major/stable or preview only if justified
- `Nullable` enabled
- `ImplicitUsings` enabled
- `TreatWarningsAsErrors` true for library builds if reasonable
- deterministic build
- XML docs enabled for library projects
- package metadata placeholders:
  - Authors: `Anthony Watts`
  - Company: blank or `Anthony Watts`
  - RepositoryUrl: `https://github.com/AnthonyPWatts/ISOCodex.Countries`
  - PackageProjectUrl: same repo URL
  - PackageLicenseExpression: `MIT`
  - RepositoryType: `git`

Make this conservative enough that test/sample projects can opt out of XML docs or warning strictness if needed.

### `Directory.Packages.props`

Create central package management with likely package versions for the first pass:

- `xunit`
- `xunit.runner.visualstudio`
- `Microsoft.NET.Test.Sdk`
- `coverlet.collector`
- `Microsoft.SourceLink.GitHub`

Use current stable versions if easily discoverable from the installed NuGet cache or existing local conventions. If not, choose sensible stable .NET 8-compatible versions.

### `AGENTS.md`

Create repository-level Codex rules.

Include:

- Work in small phases.
- Keep the core library dependency-light.
- No hidden network calls in runtime library code.
- Do not publish packages.
- Run restore/build/test/pack before claiming completion once projects exist.
- Avoid “credit-burning whirlpools”:
  - do not chase exhaustive ISO data in the first pass,
  - do not build a full localisation system,
  - do not block on perfect subdivision coverage,
  - document limitations honestly.
- Preserve UK English in docs.
- Prefer explicit, boring, stable APIs.
- Use structured validation issues rather than stringly ad-hoc errors.

### `docs/decisions/0001-project-intent.md`

Create an ADR-style note:

- Context: Addressing and Currency suggest a broader ISOCodex pattern.
- Decision: create Countries as the shared foundation for country/jurisdiction data.
- Consequences:
  - future ISOCodex packages may depend on it,
  - must be careful around data-source/version transparency,
  - must not become a political authority.

### `docs/data-sources.md`

Stub the data-source policy:

- ISO 3166 conceptual alignment.
- UN M49 numeric alignment where applicable.
- CLDR-style display-name thinking where appropriate.
- No claim of official ISO endorsement.
- First implementation may use a small safe seed dataset if redistribution/licensing is uncertain.
- Future work should improve source coverage transparently.

### `docs/design-notes.md`

Stub the design intent:

- value objects over naked strings,
- invalid vs unknown vs reserved must be distinct,
- display names are not identifiers,
- no hidden network calls,
- source/version transparency.

### `docs/versioning.md`

Stub SemVer and data-versioning guidance:

- public API changes follow SemVer,
- validation issue codes should be stable,
- data updates need to be documented,
- removing/changing canonical identifiers is breaking,
- adding metadata/subdivisions is usually non-breaking.

### `.github/workflows/build.yml`

Create a future-ready workflow that assumes the eventual solution exists.

It should:

- checkout
- setup .NET 8
- restore
- build Release
- test Release
- pack Release to `artifacts`

Because the solution may not exist yet after the bootstrap pass, either:

1. add a clearly marked TODO and allow the workflow to become active after first development pass, or
2. create the workflow with commands that will work once the `.sln` exists.

Do not contort the bootstrap just to satisfy CI before any code exists.

### `data/README.md`

Explain:

- this folder is for source-aligned seed/generated data,
- data files must include provenance/version details,
- do not add copied data unless redistribution rights are clear.

### `.codex/README.md`

Explain that Codex Desktop project/environment settings may place files under `.codex`, and that project-specific Codex setup/actions can live there if generated by the app.

Do not invent Codex config formats unless the app creates them.

## Git initialisation

If this folder is not already a Git repository:

```powershell
git init
git branch -M main
```

Add the bootstrap files and commit:

```powershell
git add .
git commit -m "Create initial ISOCodex.Countries scaffold"
```

If Git user identity is not configured and commit fails, do not fake it. Leave files staged or unstaged as appropriate and tell Tony the exact command failure.

## GitHub remote creation

If GitHub CLI is installed and authenticated, create the GitHub repo.

Check:

```powershell
gh --version
gh auth status
```

If both work, use:

```powershell
gh repo create AnthonyPWatts/ISOCodex.Countries --public --source=. --remote=origin --push
```

If Tony clearly prefers private repos from existing context or the current folder indicates private work, ask before creating the remote as public. Otherwise use public, because the current ISOCodex work appears to be public/open-source oriented.

If `gh` is unavailable, unauthenticated, or fails:

- do not thrash,
- do not try random alternatives,
- leave the local repo complete,
- provide manual instructions:

```powershell
# Create an empty repo named ISOCodex.Countries on GitHub, then:
git remote add origin https://github.com/AnthonyPWatts/ISOCodex.Countries.git
git push -u origin main
```

If SSH remotes are preferred locally, use:

```powershell
git remote add origin git@github.com:AnthonyPWatts/ISOCodex.Countries.git
git push -u origin main
```

## Final response requirements

When finished, report:

- what files/folders were created,
- whether Git was initialised,
- whether a commit was created,
- whether GitHub remote creation/push succeeded,
- if not, the exact manual steps still required,
- the recommended next prompt: run `01_FIRST_PASS_ISOCODEX_COUNTRIES.md`.

Do not start the first development pass unless Tony explicitly asks you to.
