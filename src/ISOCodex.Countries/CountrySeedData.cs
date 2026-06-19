namespace ISOCodex.Countries;

internal static class CountrySeedData
{
    public static IReadOnlyList<CountryInfo> Countries { get; } = new List<CountryInfo>
    {
        new(
            CountryAlpha2Code.Parse("GB"),
            CountryAlpha3Code.Parse("GBR"),
            CountryNumericCode.Parse("826"),
            "United Kingdom",
            "United Kingdom of Great Britain and Northern Ireland",
            CountryEntryStatus.Current,
            commonAliases: new[] { "Britain", "Great Britain" },
            notes: new[]
            {
                "GB is the canonical ISO-style alpha-2 country code used by this package.",
                "UK is commonly encountered but is not silently treated as canonical GB."
            }),
        new(CountryAlpha2Code.Parse("US"), CountryAlpha3Code.Parse("USA"), CountryNumericCode.Parse("840"), "United States", "United States of America", CountryEntryStatus.Current),
        new(CountryAlpha2Code.Parse("DE"), CountryAlpha3Code.Parse("DEU"), CountryNumericCode.Parse("276"), "Germany", "Federal Republic of Germany", CountryEntryStatus.Current),
        new(CountryAlpha2Code.Parse("FR"), CountryAlpha3Code.Parse("FRA"), CountryNumericCode.Parse("250"), "France", "French Republic", CountryEntryStatus.Current),
        new(CountryAlpha2Code.Parse("IE"), CountryAlpha3Code.Parse("IRL"), CountryNumericCode.Parse("372"), "Ireland", null, CountryEntryStatus.Current),
        new(CountryAlpha2Code.Parse("AL"), CountryAlpha3Code.Parse("ALB"), CountryNumericCode.Parse("008"), "Albania", "Republic of Albania", CountryEntryStatus.Current),
        new(CountryAlpha2Code.Parse("CA"), CountryAlpha3Code.Parse("CAN"), CountryNumericCode.Parse("124"), "Canada", null, CountryEntryStatus.Current),
        new(CountryAlpha2Code.Parse("AU"), CountryAlpha3Code.Parse("AUS"), CountryNumericCode.Parse("036"), "Australia", null, CountryEntryStatus.Current)
    }.AsReadOnly();

    public static IReadOnlyList<CountrySubdivisionInfo> Subdivisions { get; } = new List<CountrySubdivisionInfo>
    {
        new(CountrySubdivisionCode.Parse("GB-ENG"), CountryAlpha2Code.Parse("GB"), "England", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("GB-SCT"), CountryAlpha2Code.Parse("GB"), "Scotland", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("GB-WLS"), CountryAlpha2Code.Parse("GB"), "Wales", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("GB-NIR"), CountryAlpha2Code.Parse("GB"), "Northern Ireland", null, CountrySubdivisionType.Nation),
        new(CountrySubdivisionCode.Parse("US-CA"), CountryAlpha2Code.Parse("US"), "California", null, CountrySubdivisionType.State),
        new(CountrySubdivisionCode.Parse("CA-ON"), CountryAlpha2Code.Parse("CA"), "Ontario", null, CountrySubdivisionType.Province),
        new(CountrySubdivisionCode.Parse("AU-NSW"), CountryAlpha2Code.Parse("AU"), "New South Wales", null, CountrySubdivisionType.State),
        new(CountrySubdivisionCode.Parse("IE-D"), CountryAlpha2Code.Parse("IE"), "Dublin", null, CountrySubdivisionType.County)
    }.AsReadOnly();
}
