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
        Assert.False(CountryRegistry.TryGetByAlpha2("ZZ", out CountryInfo? country));
        Assert.Null(country);

        CountryCodeLookupResult result = CountryRegistry.Lookup("ZZ");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
        Assert.Equal("ZZ", result.NormalizedInput);
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

    [Theory]
    [InlineData("UK")]
    [InlineData("EU")]
    [InlineData("ZZ")]
    public void Special_Or_User_Assigned_Shaped_Codes_Are_Unknown_Not_Reserved_Statuses(string input)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.False(result.Success);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
        Assert.Equal(input, result.NormalizedInput);
    }

    [Fact]
    public void Lookup_Invalid_Syntax_Does_Not_Throw()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("1!");

        Assert.False(result.Success);
        Assert.Null(result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.InvalidSyntax, result.FailureReason);
    }

    [Fact]
    public void Subdivision_Registry_Returns_Known_And_Unknown_Cases()
    {
        Assert.True(CountrySubdivisionRegistry.TryGetByCode("GB-ENG", out CountrySubdivisionInfo? subdivision));
        Assert.Equal("England", subdivision?.EnglishName);

        Assert.False(CountrySubdivisionRegistry.TryGetByCode("GB-XYZ", out CountrySubdivisionInfo? unknown));
        Assert.Null(unknown);
    }
}
