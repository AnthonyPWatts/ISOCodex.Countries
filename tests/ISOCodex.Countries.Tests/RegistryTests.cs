namespace ISOCodex.Countries.Tests;

public sealed class RegistryTests
{
    [Fact]
    public void Lookup_Gb_By_Alpha2_Returns_United_Kingdom()
    {
        CountryInfo country = CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse("GB"));

        Assert.Equal("United Kingdom", country.EnglishShortName);
    }

    [Fact]
    public void Lookup_Gbr_And_826_Return_Same_Entry_As_Gb()
    {
        CountryInfo alpha2 = CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse("GB"));
        CountryInfo alpha3 = CountryRegistry.GetByAlpha3(CountryAlpha3Code.Parse("GBR"));
        CountryInfo numeric = CountryRegistry.GetByNumeric(CountryNumericCode.Parse("826"));

        Assert.Same(alpha2, alpha3);
        Assert.Same(alpha2, numeric);
    }

    [Fact]
    public void Lookup_008_Preserves_Albania_Numeric_Code()
    {
        CountryInfo country = CountryRegistry.GetByNumeric(CountryNumericCode.Parse("008"));

        Assert.Equal("Albania", country.EnglishShortName);
        Assert.Equal("008", country.Numeric.ToString());
    }

    [Fact]
    public void Unknown_Syntax_Valid_Code_Returns_False_And_Result_Failure()
    {
        Assert.False(CountryRegistry.TryGetByAlpha2("AB", out CountryInfo? country));
        Assert.Null(country);

        CountryCodeLookupResult result = CountryRegistry.Lookup("AB");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
        Assert.Equal("AB", result.NormalizedInput);
    }

    [Fact]
    public void GetBy_Throws_Clearly_For_Unknown_Codes()
    {
        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(
            () => CountryRegistry.GetByAlpha2(CountryAlpha2Code.Parse("ZZ")));

        Assert.Contains("ZZ", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1!")]
    [InlineData("GBR1")]
    public void TryGetBy_Handles_Invalid_Input_Safely(string? input)
    {
        Assert.False(CountryRegistry.TryGetByAlpha2(input, out _));
        Assert.False(CountryRegistry.TryGetByAlpha3(input, out _));
        Assert.False(CountryRegistry.TryGetByNumeric(input, out _));
    }

    [Theory]
    [InlineData("GB", CountryCodeKind.Alpha2, "United Kingdom")]
    [InlineData("gbr", CountryCodeKind.Alpha3, "United Kingdom")]
    [InlineData("826", CountryCodeKind.Numeric, "United Kingdom")]
    public void Mixed_Lookup_Detects_Code_Kind(string input, CountryCodeKind expectedKind, string expectedName)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.True(result.Success);
        Assert.Equal(expectedKind, result.DetectedKind);
        Assert.Equal(expectedName, result.Country?.EnglishShortName);
    }

    [Fact]
    public void Uk_Is_Not_Silently_Treated_As_Gb()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("UK");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeLookupFailureReason.ReservedButNotCountry, result.FailureReason);
    }

    [Theory]
    [InlineData("UK")]
    [InlineData("EU")]
    [InlineData("ZZ")]
    public void Reserved_Or_Special_Alpha2_Codes_Are_Not_Silently_Treated_As_Countries(string input)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.ReservedButNotCountry, result.FailureReason);
    }
}
