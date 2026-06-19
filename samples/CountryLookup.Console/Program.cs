using ISOCodex.Countries;

Console.WriteLine("ISOCodex.Countries lookup sample");
Console.WriteLine();

CountryAlpha2Code alpha2 = CountryAlpha2Code.Parse("gb");
CountryAlpha3Code alpha3 = CountryAlpha3Code.Parse("gbr");
CountryNumericCode numeric = CountryNumericCode.Parse("826");

Console.WriteLine("Parsed alpha-2: " + alpha2);
Console.WriteLine("Parsed alpha-3: " + alpha3);
Console.WriteLine("Parsed numeric: " + numeric);
Console.WriteLine();

foreach (string input in new[] { "GB", "gbr", "826", "008", "UK", "EU", "XK", "ZZ", "12!" })
{
    CountryCodeLookupResult result = CountryRegistry.Lookup(input);

    if (result.Success && result.Country is not null)
    {
        CountryInfo country = result.Country;
        Console.WriteLine(input + " -> " + country.Alpha2 + " / " + country.Alpha3 + " / " + country.Numeric + " - " + country.EnglishShortName);
        continue;
    }

    Console.WriteLine(input + " -> not found (" + result.FailureReason + ")");
}
