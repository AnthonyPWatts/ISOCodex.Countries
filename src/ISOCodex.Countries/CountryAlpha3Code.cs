namespace ISOCodex.Countries;

/// <summary>
/// Represents a canonical ISO-style three-letter country code.
/// </summary>
public readonly struct CountryAlpha3Code : IEquatable<CountryAlpha3Code>, IComparable<CountryAlpha3Code>, IComparable
{
    private readonly string? _value;

    private CountryAlpha3Code(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical uppercase code value.
    /// </summary>
    public string Value => _value ?? string.Empty;

    /// <summary>
    /// Parses a three-letter country code.
    /// </summary>
    public static CountryAlpha3Code Parse(string value)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out string normalized);

        if (issue is not null)
        {
            throw new FormatException(issue.Message);
        }

        return new CountryAlpha3Code(normalized);
    }

    /// <summary>
    /// Attempts to parse a three-letter country code.
    /// </summary>
    public static bool TryParse(string? value, out CountryAlpha3Code code)
    {
        CountryCodeValidationIssue? issue = TryValidate(value, out string normalized);

        if (issue is not null)
        {
            code = default;
            return false;
        }

        code = new CountryAlpha3Code(normalized);
        return true;
    }

    /// <summary>
    /// Returns whether the supplied value has valid alpha-3 syntax.
    /// </summary>
    public static bool IsValidSyntax(string? value) => TryValidate(value, out _) is null;

    /// <summary>
    /// Attempts to validate alpha-3 syntax and returns a structured issue when invalid.
    /// </summary>
    public static CountryCodeValidationIssue? TryValidate(string? value, out string normalized) =>
        CountryCodeSyntax.ValidateAlphaCode(value, 3, "country.alpha3", "Country alpha-3 code", out normalized);

    /// <inheritdoc />
    public bool Equals(CountryAlpha3Code other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CountryAlpha3Code other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc />
    public int CompareTo(CountryAlpha3Code other) => StringComparer.Ordinal.Compare(Value, other.Value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is CountryAlpha3Code other)
        {
            return CompareTo(other);
        }

        throw new ArgumentException("Object must be a CountryAlpha3Code.", nameof(obj));
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    public static bool operator ==(CountryAlpha3Code left, CountryAlpha3Code right) => left.Equals(right);

    public static bool operator !=(CountryAlpha3Code left, CountryAlpha3Code right) => !left.Equals(right);
}
