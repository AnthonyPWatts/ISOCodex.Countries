namespace ISOCodex.Countries;

/// <summary>
/// Represents a country-code-shaped element, including non-country special elements.
/// </summary>
public sealed class CountryCodeElementInfo
{
    /// <summary>
    /// Creates a country-code element entry.
    /// </summary>
    public CountryCodeElementInfo(
        CountryAlpha2Code alpha2,
        CountryCodeElementKind kind,
        string displayName,
        string source,
        string? notes)
    {
        Alpha2 = alpha2;
        Kind = kind;
        DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? throw new ArgumentException("Display name is required.", nameof(displayName))
            : displayName;
        Source = string.IsNullOrWhiteSpace(source)
            ? throw new ArgumentException("Source is required.", nameof(source))
            : source;
        Notes = notes;
    }

    public CountryAlpha2Code Alpha2 { get; }

    public CountryCodeElementKind Kind { get; }

    public string DisplayName { get; }

    public string Source { get; }

    public string? Notes { get; }
}
