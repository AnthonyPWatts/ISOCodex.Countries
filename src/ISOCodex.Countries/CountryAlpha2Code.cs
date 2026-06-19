namespace ISOCodex.Countries;

/// <summary>
/// Represents a canonical ISO-style two-letter country code.
/// </summary>
public readonly struct CountryAlpha2Code : IEquatable<CountryAlpha2Code>, IComparable<CountryAlpha2Code>, IComparable
{
    private readonly string? _value;

    private CountryAlpha2Code(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical uppercase code value.
    /// </summary>
    public string Value => _value ?? string.Empty;

    /// <summary>
    /// Parses a two-letter country code.
    /// </summary>
    public static CountryAlpha2Code Parse(string value)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out string normalized);

        if (issue is not null)
        {
            throw new FormatException(issue.Message);
        }

        return new CountryAlpha2Code(normalized);
    }

    /// <summary>
    /// Attempts to parse a two-letter country code.
    /// </summary>
    public static bool TryParse(string? value, out CountryAlpha2Code code)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out string normalized);

        if (issue is not null)
        {
            code = default;
            return false;
        }

        code = new CountryAlpha2Code(normalized);
        return true;
    }

    /// <summary>
    /// Returns whether the supplied value has valid alpha-2 syntax.
    /// </summary>
    public static bool IsValidSyntax(string? value) => TryValidate(value, out _) is null;

    /// <summary>
    /// Attempts to validate alpha-2 syntax and returns a structured issue when invalid.
    /// </summary>
    public static CountryCodeValidationIssue? TryValidate(string? value, out string normalized) =>
        CountryCodeSyntax.ValidateAlphaCode(value, 2, "country.alpha2", "Country alpha-2 code", out normalized);

    /// <inheritdoc />
    public bool Equals(CountryAlpha2Code other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CountryAlpha2Code other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc />
    public int CompareTo(CountryAlpha2Code other) => StringComparer.Ordinal.Compare(Value, other.Value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is CountryAlpha2Code other)
        {
            return CompareTo(other);
        }

        throw new ArgumentException("Object must be a CountryAlpha2Code.", nameof(obj));
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    public static bool operator ==(CountryAlpha2Code left, CountryAlpha2Code right) => left.Equals(right);

    public static bool operator !=(CountryAlpha2Code left, CountryAlpha2Code right) => !left.Equals(right);

    public static bool operator <(CountryAlpha2Code left, CountryAlpha2Code right) => left.CompareTo(right) < 0;

    public static bool operator >(CountryAlpha2Code left, CountryAlpha2Code right) => left.CompareTo(right) > 0;
}
