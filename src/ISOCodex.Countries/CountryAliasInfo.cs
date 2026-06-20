using System.Text;

namespace ISOCodex.Countries;

/// <summary>
/// Represents an explicit opt-in country alias.
/// </summary>
public sealed class CountryAliasInfo
{
    /// <summary>
    /// Creates a country alias entry.
    /// </summary>
    public CountryAliasInfo(
        string alias,
        CountryAlpha2Code? replacementCountryCode,
        CountryAliasKind kind,
        string source,
        string? notes)
    {
        Alias = string.IsNullOrWhiteSpace(alias)
            ? throw new ArgumentException("Alias is required.", nameof(alias))
            : alias.Normalize(NormalizationForm.FormC);
        ReplacementCountryCode = replacementCountryCode;
        Kind = kind;
        Source = string.IsNullOrWhiteSpace(source)
            ? throw new ArgumentException("Alias source is required.", nameof(source))
            : source;
        Notes = notes;
    }

    public string Alias { get; }

    public CountryAlpha2Code? ReplacementCountryCode { get; }

    public CountryAliasKind Kind { get; }

    public string Source { get; }

    public string? Notes { get; }
}
