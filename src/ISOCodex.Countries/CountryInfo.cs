namespace ISOCodex.Countries;

/// <summary>
/// Represents a known country or territory entry.
/// </summary>
public sealed class CountryInfo
{
    /// <summary>
    /// Creates a country entry.
    /// </summary>
    public CountryInfo(
        CountryAlpha2Code alpha2,
        CountryAlpha3Code alpha3,
        CountryNumericCode numeric,
        string englishShortName,
        string? englishOfficialName,
        CountryEntryStatus status,
        IEnumerable<string>? commonAliases = null,
        IEnumerable<string>? notes = null)
    {
        Alpha2 = alpha2;
        Alpha3 = alpha3;
        Numeric = numeric;
        EnglishShortName = string.IsNullOrWhiteSpace(englishShortName)
            ? throw new ArgumentException("English short name is required.", nameof(englishShortName))
            : englishShortName;
        EnglishOfficialName = englishOfficialName;
        Status = status;
        CommonAliases = new List<string>(commonAliases ?? Array.Empty<string>()).AsReadOnly();
        Notes = new List<string>(notes ?? Array.Empty<string>()).AsReadOnly();
    }

    public CountryAlpha2Code Alpha2 { get; }

    public CountryAlpha3Code Alpha3 { get; }

    public CountryNumericCode Numeric { get; }

    public string EnglishShortName { get; }

    public string? EnglishOfficialName { get; }

    public CountryEntryStatus Status { get; }

    public IReadOnlyList<string> CommonAliases { get; }

    public IReadOnlyList<string> Notes { get; }

    /// <inheritdoc />
    public override string ToString() => Alpha2 + " - " + EnglishShortName;
}
