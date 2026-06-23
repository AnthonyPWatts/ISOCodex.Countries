namespace ISOCodex.Countries;

/// <summary>
/// Represents a canonical ISO 3166-2-style subdivision code.
/// </summary>
public readonly struct CountrySubdivisionCode : IEquatable<CountrySubdivisionCode>, IComparable<CountrySubdivisionCode>, IComparable
{
    private readonly string? _value;
    private readonly string? _subdivisionPart;

    private CountrySubdivisionCode(CountryAlpha2Code countryCode, string subdivisionPart)
    {
        CountryCode = countryCode;
        _subdivisionPart = subdivisionPart;
        _value = countryCode.ToString() + "-" + subdivisionPart;
    }

    /// <summary>
    /// Gets the country-code prefix.
    /// </summary>
    public CountryAlpha2Code CountryCode { get; }

    /// <summary>
    /// Gets the subdivision part after the hyphen.
    /// </summary>
    public string SubdivisionPart => _subdivisionPart ?? string.Empty;

    /// <summary>
    /// Gets the canonical subdivision code.
    /// </summary>
    public string Value => _value ?? string.Empty;

    /// <summary>
    /// Parses a subdivision code.
    /// </summary>
    public static CountrySubdivisionCode Parse(string value)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out CountryAlpha2Code countryCode, out string subdivisionPart);

        if (issue is not null)
        {
            throw new FormatException(issue.Message);
        }

        return new CountrySubdivisionCode(countryCode, subdivisionPart);
    }

    /// <summary>
    /// Attempts to parse a subdivision code.
    /// </summary>
    public static bool TryParse(string? value, out CountrySubdivisionCode code)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out CountryAlpha2Code countryCode, out string subdivisionPart);

        if (issue is not null)
        {
            code = default;
            return false;
        }

        code = new CountrySubdivisionCode(countryCode, subdivisionPart);
        return true;
    }

    /// <summary>
    /// Returns whether the supplied value has valid subdivision-code syntax.
    /// </summary>
    public static bool IsValidSyntax(string? value) => TryValidate(value, out _, out _) is null;

    /// <summary>
    /// Attempts to validate subdivision-code syntax and returns a structured issue when invalid.
    /// </summary>
    public static CountryCodeValidationIssue? TryValidate(string? value, out CountryAlpha2Code countryCode, out string subdivisionPart)
    {
        countryCode = default;
        subdivisionPart = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return InvalidFormat(value);
        }

        string[] parts = value.Split('-');

        if (parts.Length != 2 || !CountryAlpha2Code.TryParse(parts[0], out countryCode))
        {
            return InvalidFormat(value);
        }

        if (parts[1].Length < 1 || parts[1].Length > 3)
        {
            return InvalidFormat(value);
        }

        for (int i = 0; i < parts[1].Length; i++)
        {
            char c = parts[1][i];
            bool isAsciiLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
            bool isDigit = c >= '0' && c <= '9';

            if (!isAsciiLetter && !isDigit)
            {
                return InvalidFormat(value);
            }
        }

        subdivisionPart = parts[1].ToUpperInvariant();
        return null;
    }

    private static CountryCodeValidationIssue InvalidFormat(string? value) =>
        new("country.subdivision.invalid_format", "Country subdivision code must use the form AA-XXX.", value);

    /// <inheritdoc />
    public bool Equals(CountrySubdivisionCode other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CountrySubdivisionCode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc />
    public int CompareTo(CountrySubdivisionCode other) => StringComparer.Ordinal.Compare(Value, other.Value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is CountrySubdivisionCode other)
        {
            return CompareTo(other);
        }

        throw new ArgumentException("Object must be a CountrySubdivisionCode.", nameof(obj));
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    public static bool operator ==(CountrySubdivisionCode left, CountrySubdivisionCode right) => left.Equals(right);

    public static bool operator !=(CountrySubdivisionCode left, CountrySubdivisionCode right) => !left.Equals(right);

    public static bool operator <(CountrySubdivisionCode left, CountrySubdivisionCode right) => left.CompareTo(right) < 0;

    public static bool operator >(CountrySubdivisionCode left, CountrySubdivisionCode right) => left.CompareTo(right) > 0;
}
