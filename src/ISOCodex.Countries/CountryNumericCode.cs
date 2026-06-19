namespace ISOCodex.Countries;

/// <summary>
/// Represents a canonical three-digit country numeric code.
/// </summary>
public readonly struct CountryNumericCode : IEquatable<CountryNumericCode>, IComparable<CountryNumericCode>, IComparable
{
    private readonly string? _value;

    private CountryNumericCode(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical three-digit code value.
    /// </summary>
    public string Value => _value ?? string.Empty;

    /// <summary>
    /// Parses a three-digit country numeric code.
    /// </summary>
    public static CountryNumericCode Parse(string value)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out string normalized);

        if (issue is not null)
        {
            throw new FormatException(issue.Message);
        }

        return new CountryNumericCode(normalized);
    }

    /// <summary>
    /// Creates a numeric code from an integer, preserving canonical leading zeroes.
    /// </summary>
    public static CountryNumericCode FromInt32(int value)
    {
        if (value < 0 || value > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Country numeric code must be between 0 and 999.");
        }

        return new CountryNumericCode(value.ToString("D3", System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Attempts to parse a three-digit country numeric code.
    /// </summary>
    public static bool TryParse(string? value, out CountryNumericCode code)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out string normalized);

        if (issue is not null)
        {
            code = default;
            return false;
        }

        code = new CountryNumericCode(normalized);
        return true;
    }

    /// <summary>
    /// Returns whether the supplied value has valid numeric-code syntax.
    /// </summary>
    public static bool IsValidSyntax(string? value) => TryValidate(value, out _) is null;

    /// <summary>
    /// Attempts to validate numeric-code syntax and returns a structured issue when invalid.
    /// </summary>
    public static CountryCodeValidationIssue? TryValidate(string? value, out string normalized) =>
        CountryCodeSyntax.ValidateNumericCode(value, out normalized);

    /// <inheritdoc />
    public bool Equals(CountryNumericCode other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CountryNumericCode other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc />
    public int CompareTo(CountryNumericCode other) => StringComparer.Ordinal.Compare(Value, other.Value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is CountryNumericCode other)
        {
            return CompareTo(other);
        }

        throw new ArgumentException("Object must be a CountryNumericCode.", nameof(obj));
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    public static bool operator ==(CountryNumericCode left, CountryNumericCode right) => left.Equals(right);

    public static bool operator !=(CountryNumericCode left, CountryNumericCode right) => !left.Equals(right);

    public static bool operator <(CountryNumericCode left, CountryNumericCode right) => left.CompareTo(right) < 0;

    public static bool operator >(CountryNumericCode left, CountryNumericCode right) => left.CompareTo(right) > 0;
}
