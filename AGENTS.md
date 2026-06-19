# Repository Instructions

This repository is for `ISOCodex.Countries`, a dependency-light .NET foundation library for country, country-code, subdivision, and jurisdiction metadata.

Work in small phases. Prefer explicit, boring, stable APIs over clever abstraction. Keep the core library dependency-light and avoid hidden network calls in runtime library code.

Do not publish packages from this repository unless explicitly asked. Once projects exist, run restore, build, test, and pack before claiming completion.

Avoid credit-burning whirlpools:

- Do not chase exhaustive ISO data in the first pass.
- Do not build a full localisation system.
- Do not block on perfect subdivision coverage.
- Document limitations honestly.

Use UK English in documentation. Use structured validation issues rather than stringly ad-hoc errors.
