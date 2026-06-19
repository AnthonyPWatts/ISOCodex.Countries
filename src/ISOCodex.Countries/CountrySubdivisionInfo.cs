namespace ISOCodex.Countries;

/// <summary>
/// Represents a known country subdivision entry.
/// </summary>
public sealed class CountrySubdivisionInfo
{
    /// <summary>
    /// Creates a subdivision entry.
    /// </summary>
    public CountrySubdivisionInfo(
        CountrySubdivisionCode code,
        CountryAlpha2Code countryCode,
        string englishName,
        string? localName,
        CountrySubdivisionType type)
    {
        Code = code;
        CountryCode = countryCode;
        EnglishName = string.IsNullOrWhiteSpace(englishName)
            ? throw new ArgumentException("English name is required.", nameof(englishName))
            : englishName;
        LocalName = localName;
        Type = type;
    }

    public CountrySubdivisionCode Code { get; }

    public CountryAlpha2Code CountryCode { get; }

    public string EnglishName { get; }

    public string? LocalName { get; }

    public CountrySubdivisionType Type { get; }

    /// <inheritdoc />
    public override string ToString() => Code + " - " + EnglishName;
}
