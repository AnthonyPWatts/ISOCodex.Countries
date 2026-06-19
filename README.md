# ISOCodex.Countries

`ISOCodex.Countries` is a .NET foundation library for strongly typed country, country-code, subdivision, and jurisdiction metadata.

It is intended for normal business applications that need stable value objects, syntax validation, canonical formatting, registry lookup, and explicit data-source limitations.

It is not an official ISO product, is not endorsed by ISO, and is not a geopolitical authority.

## Current Status

Hardened v0.x foundation. The package has a small representative seed dataset and a stable foundation API, but it is not data-complete.

## Installation

The package is not published yet. After cloning the repository, build and pack locally:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release --output ./artifacts
```

## Quick Start

```csharp
using ISOCodex.Countries;

CountryAlpha2Code countryCode = CountryAlpha2Code.Parse("gb");
CountryInfo country = CountryRegistry.GetByAlpha2(countryCode);

Console.WriteLine(country.Alpha2);            // GB
Console.WriteLine(country.EnglishShortName);  // United Kingdom
```

For boundary input, prefer `TryParse` or `CountryRegistry.Lookup`:

```csharp
CountryCodeLookupResult result = CountryRegistry.Lookup("826");

if (result.Success)
{
    Console.WriteLine(result.Country!.EnglishShortName);
}
```

## Country Codes

The v0.1 foundation supports value objects for:

- `CountryAlpha2Code`, such as `GB`
- `CountryAlpha3Code`, such as `GBR`
- `CountryNumericCode`, such as `826` or `008`

Alpha codes are stored in canonical uppercase. Numeric codes are stored as three-character strings so leading zeroes are preserved.

Mixed lookup is deterministic: two ASCII letters are treated as alpha-2, three ASCII letters as alpha-3, and three digits as numeric. `UK` is recognised as syntactically valid alpha-2 input, but it is not silently treated as canonical `GB`.

`EU` and `ZZ` are also syntactically valid alpha-2 shapes, but they are not current countries in the representative registry and are returned as `Unknown` by mixed lookup.

## Persistence Guidance

For most applications, store canonical alpha-2 codes such as `GB`.

Store alpha-3 or numeric codes only when an integration requires them.

Do not store display names as identifiers.

If preserving what a user originally typed matters, store the original input separately from the canonical code.

## Subdivision Codes

`CountrySubdivisionCode` models ISO 3166-2-style codes such as `GB-ENG`, `US-CA`, and `IE-D`.

Syntax validity does not mean the subdivision is known. Use `CountrySubdivisionRegistry.TryGetByCode` when you need to check against the representative seed registry.

## JSON

Converters are included for the value objects:

```csharp
using System.Text.Json;
using ISOCodex.Countries;

var options = new JsonSerializerOptions();
options.Converters.Add(new CountryAlpha2CodeJsonConverter());
options.Converters.Add(new CountryAlpha3CodeJsonConverter());
options.Converters.Add(new CountryNumericCodeJsonConverter());
options.Converters.Add(new CountrySubdivisionCodeJsonConverter());

string json = JsonSerializer.Serialize(CountryAlpha2Code.Parse("gb"), options);
CountryAlpha2Code code = JsonSerializer.Deserialize<CountryAlpha2Code>("\"GB\"", options);
```

Converters serialise to canonical strings and reject invalid values during deserialisation.

Default value objects serialise as empty strings. A default value object is not a meaningful canonical country code; treat it as an uninitialised value rather than persisted domain data.

## CSV Import Validation

For import pipelines, keep row-level errors rather than throwing for normal invalid input:

```csharp
foreach (string input in countryColumnValues)
{
    CountryCodeLookupResult result = CountryRegistry.Lookup(input);

    if (!result.Success)
    {
        // Store result.FailureReason against the row.
        continue;
    }

    CountryAlpha2Code canonicalCode = result.Country!.Alpha2;
}
```

See `samples/CsvImport.Validation` for a complete compiling example.

## Country Dropdowns

For simple dropdowns, use `CountryRegistry.All` and store `CountryInfo.Alpha2` as the canonical value. Display names are useful labels, not identifiers.

```csharp
var options = CountryRegistry.All
    .OrderBy(country => country.EnglishShortName)
    .Select(country => new { Value = country.Alpha2.Value, Label = country.EnglishShortName });
```

## Data Sources And Limitations

The seed data is aligned with ISO 3166 and UN M49 concepts, but it is a small hand-curated representative dataset only. It currently includes GB, US, DE, FR, IE, AL, CA, and AU, plus representative subdivisions for those examples. It is not a redistributed official ISO dataset.

Current data version:

```csharp
Console.WriteLine(CountryDataVersion.Identifier);
Console.WriteLine(CountryDataVersion.CheckedOn);
Console.WriteLine(CountryDataVersion.Description);
```

No runtime code makes hidden network calls. The runtime package uses checked-in compiled seed data rather than loose external files.

Data-source policy and limitations are documented in [`docs/data-sources.md`](docs/data-sources.md). The v1.0 data decision is tracked in [`docs/data-strategy.md`](docs/data-strategy.md).

## Release Verification

Before any release, use [`docs/release-gate.md`](docs/release-gate.md). The local package smoke test is:

```powershell
./eng/smoke-test-package.ps1
```

## Contributing Data Corrections

Data corrections should include the source used, the date checked, and whether the change affects a canonical identifier, display metadata, aliases, notes, or subdivision coverage.

Changing or removing canonical identifiers is breaking. Adding metadata or representative subdivisions is usually non-breaking, but should still be documented.
