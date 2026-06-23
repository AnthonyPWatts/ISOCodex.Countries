namespace ISOCodex.Countries;

/// <summary>
/// Provides lookup access to the checked-in country seed registry.
/// </summary>
public static class CountryRegistry
{
    private static readonly HashSet<string> ReservedOrSpecialAlpha2Codes = new(StringComparer.Ordinal)
    {
        "EU",
        "UK",
        "ZZ"
    };

    public static IReadOnlyList<CountryInfo> All { get; } = CountrySeedData.Countries;

    private static readonly IReadOnlyDictionary<string, CountryInfo> ByAlpha2 =
        All.ToDictionary(country => country.Alpha2.Value, StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, CountryInfo> ByAlpha3 =
        All.ToDictionary(country => country.Alpha3.Value, StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, CountryInfo> ByNumeric =
        All.ToDictionary(country => country.Numeric.Value, StringComparer.Ordinal);

    public static CountryInfo GetByAlpha2(CountryAlpha2Code code) =>
        TryGetByAlpha2(code, out CountryInfo? country)
            ? country!
            : throw new KeyNotFoundException("No country is known for alpha-2 code '" + code + "'.");

    public static CountryInfo GetByAlpha3(CountryAlpha3Code code) =>
        TryGetByAlpha3(code, out CountryInfo? country)
            ? country!
            : throw new KeyNotFoundException("No country is known for alpha-3 code '" + code + "'.");

    public static CountryInfo GetByNumeric(CountryNumericCode code) =>
        TryGetByNumeric(code, out CountryInfo? country)
            ? country!
            : throw new KeyNotFoundException("No country is known for numeric code '" + code + "'.");

    public static bool TryGetByAlpha2(CountryAlpha2Code code, out CountryInfo? country) =>
        ByAlpha2.TryGetValue(code.Value, out country);

    public static bool TryGetByAlpha3(CountryAlpha3Code code, out CountryInfo? country) =>
        ByAlpha3.TryGetValue(code.Value, out country);

    public static bool TryGetByNumeric(CountryNumericCode code, out CountryInfo? country) =>
        ByNumeric.TryGetValue(code.Value, out country);

    public static bool TryGetByAlpha2(string? value, out CountryInfo? country)
    {
        country = null;
        return CountryAlpha2Code.TryParse(value, out CountryAlpha2Code code) && TryGetByAlpha2(code, out country);
    }

    public static bool TryGetByAlpha3(string? value, out CountryInfo? country)
    {
        country = null;
        return CountryAlpha3Code.TryParse(value, out CountryAlpha3Code code) && TryGetByAlpha3(code, out country);
    }

    public static bool TryGetByNumeric(string? value, out CountryInfo? country)
    {
        country = null;
        return CountryNumericCode.TryParse(value, out CountryNumericCode code) && TryGetByNumeric(code, out country);
    }

    public static CountryCodeLookupResult Lookup(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return CountryCodeLookupResult.Failed(CountryCodeLookupFailureReason.Empty);
        }

        if (CountrySubdivisionCode.TryParse(value, out CountrySubdivisionCode subdivisionCode))
        {
            return CountryCodeLookupResult.Failed(
                CountryCodeLookupFailureReason.Unsupported,
                normalizedInput: subdivisionCode.Value);
        }

        if (CountryAlpha2Code.TryParse(value, out CountryAlpha2Code alpha2))
        {
            if (CountryCodeElementRegistry.TryGetByAlpha2(alpha2, out CountryCodeElementInfo? element)
                && element!.Kind != CountryCodeElementKind.CurrentCountry)
            {
                return CountryCodeLookupResult.Failed(
                    CountryCodeLookupFailureReason.ReservedButNotCountry,
                    CountryCodeKind.Alpha2,
                    alpha2.Value);
            }

            return TryGetByAlpha2(alpha2, out CountryInfo? country)
                ? CountryCodeLookupResult.Found(country!, CountryCodeKind.Alpha2, alpha2.Value)
                : CountryCodeLookupResult.Failed(GetAlpha2FailureReason(alpha2), CountryCodeKind.Alpha2, alpha2.Value);
        }

        if (CountryAlpha3Code.TryParse(value, out CountryAlpha3Code alpha3))
        {
            return TryGetByAlpha3(alpha3, out CountryInfo? country)
                ? CountryCodeLookupResult.Found(country!, CountryCodeKind.Alpha3, alpha3.Value)
                : CountryCodeLookupResult.Failed(CountryCodeLookupFailureReason.Unknown, CountryCodeKind.Alpha3, alpha3.Value);
        }

        if (CountryNumericCode.TryParse(value, out CountryNumericCode numeric))
        {
            return TryGetByNumeric(numeric, out CountryInfo? country)
                ? CountryCodeLookupResult.Found(country!, CountryCodeKind.Numeric, numeric.Value)
                : CountryCodeLookupResult.Failed(CountryCodeLookupFailureReason.Unknown, CountryCodeKind.Numeric, numeric.Value);
        }

        return CountryCodeLookupResult.Failed(CountryCodeLookupFailureReason.InvalidSyntax);
    }

    private static CountryCodeLookupFailureReason GetAlpha2FailureReason(CountryAlpha2Code code) =>
        ReservedOrSpecialAlpha2Codes.Contains(code.Value)
            ? CountryCodeLookupFailureReason.ReservedButNotCountry
            : CountryCodeLookupFailureReason.Unknown;
}
