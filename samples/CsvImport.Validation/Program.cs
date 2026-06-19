using ISOCodex.Countries;

var rows = new[]
{
    new ImportRow(1, "Acme Ltd", "GB"),
    new ImportRow(2, "Example Inc", "usa"),
    new ImportRow(3, "Contoso GmbH", "276"),
    new ImportRow(4, "Bad Syntax Co", "1!"),
    new ImportRow(5, "Special Code Co", "ZZ"),
    new ImportRow(6, "Alias-Like Co", "UK"),
    new ImportRow(7, "Kosovo Example Co", "XK")
};

var accepted = new List<string>();
var errors = new List<string>();

foreach (ImportRow row in rows)
{
    CountryCodeLookupResult lookup = CountryRegistry.Lookup(row.CountryInput);

    if (lookup.Success && lookup.Country is not null)
    {
        accepted.Add("Row " + row.RowNumber + ": " + row.CustomerName + " -> " + lookup.Country.Alpha2 + " (" + lookup.Country.EnglishShortName + ")");
        continue;
    }

    errors.Add("Row " + row.RowNumber + ": country '" + row.CountryInput + "' failed validation: " + lookup.FailureReason);
}

Console.WriteLine("Accepted rows");
foreach (string line in accepted)
{
    Console.WriteLine(line);
}

Console.WriteLine();
Console.WriteLine("Row-level errors");
foreach (string line in errors)
{
    Console.WriteLine(line);
}

internal sealed record ImportRow(int RowNumber, string CustomerName, string CountryInput);
