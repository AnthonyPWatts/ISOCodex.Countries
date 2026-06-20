namespace ISOCodex.Countries;

/// <summary>
/// Provides lookup access to country-code-shaped elements, including special non-country values.
/// </summary>
public static class CountryCodeElementRegistry
{
    public static IReadOnlyList<CountryCodeElementInfo> All { get; } = CountrySeedData.CountryCodeElements;

    private static readonly IReadOnlyDictionary<string, CountryCodeElementInfo> ByAlpha2 =
        All.ToDictionary(element => element.Alpha2.Value, StringComparer.Ordinal);

    public static bool TryGetByAlpha2(CountryAlpha2Code code, out CountryCodeElementInfo? element) =>
        ByAlpha2.TryGetValue(code.Value, out element);

    public static bool TryGetByAlpha2(string? value, out CountryCodeElementInfo? element)
    {
        element = null;
        return CountryAlpha2Code.TryParse(value, out CountryAlpha2Code code) && TryGetByAlpha2(code, out element);
    }
}
