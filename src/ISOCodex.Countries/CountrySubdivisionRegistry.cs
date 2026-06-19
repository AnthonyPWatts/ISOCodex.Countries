namespace ISOCodex.Countries;

/// <summary>
/// Provides lookup access to the checked-in subdivision seed registry.
/// </summary>
public static class CountrySubdivisionRegistry
{
    public static IReadOnlyList<CountrySubdivisionInfo> All { get; } = CountrySeedData.Subdivisions;

    private static readonly IReadOnlyDictionary<string, CountrySubdivisionInfo> ByCode =
        All.ToDictionary(subdivision => subdivision.Code.Value, StringComparer.Ordinal);

    public static CountrySubdivisionInfo GetByCode(CountrySubdivisionCode code) =>
        TryGetByCode(code, out CountrySubdivisionInfo? subdivision)
            ? subdivision!
            : throw new KeyNotFoundException("No country subdivision is known for code '" + code + "'.");

    public static bool TryGetByCode(CountrySubdivisionCode code, out CountrySubdivisionInfo? subdivision) =>
        ByCode.TryGetValue(code.Value, out subdivision);

    public static bool TryGetByCode(string? value, out CountrySubdivisionInfo? subdivision)
    {
        subdivision = null;
        return CountrySubdivisionCode.TryParse(value, out CountrySubdivisionCode code) && TryGetByCode(code, out subdivision);
    }
}
