using System.Text;

namespace ISOCodex.Countries;

/// <summary>
/// Represents a locale-specific country display name.
/// </summary>
public sealed class CountryDisplayName
{
    /// <summary>
    /// Creates a country display-name entry.
    /// </summary>
    public CountryDisplayName(
        CountryAlpha2Code countryCode,
        string languageTag,
        string name,
        CountryDisplayNameKind kind,
        bool isEndonym,
        bool isRightToLeft)
    {
        CountryCode = countryCode;
        LanguageTag = string.IsNullOrWhiteSpace(languageTag)
            ? throw new ArgumentException("Language tag is required.", nameof(languageTag))
            : languageTag;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Country display name is required.", nameof(name))
            : name.Normalize(NormalizationForm.FormC);
        Kind = kind;
        IsEndonym = isEndonym;
        IsRightToLeft = isRightToLeft;
    }

    public CountryAlpha2Code CountryCode { get; }

    public string LanguageTag { get; }

    public string Name { get; }

    public CountryDisplayNameKind Kind { get; }

    public bool IsEndonym { get; }

    public bool IsRightToLeft { get; }

    /// <inheritdoc />
    public override string ToString() => LanguageTag + ":" + CountryCode + " - " + Name;
}
