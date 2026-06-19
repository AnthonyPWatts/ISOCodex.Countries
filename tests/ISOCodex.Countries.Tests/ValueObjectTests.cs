namespace ISOCodex.Countries.Tests;

public sealed class ValueObjectTests
{
    [Theory]
    [InlineData("GB", "GB")]
    [InlineData("gb", "GB")]
    public void Alpha2_Parses_Uppercase_And_Lowercase(string input, string expected)
    {
        CountryAlpha2Code code = CountryAlpha2Code.Parse(input);

        Assert.Equal(expected, code.ToString());
        Assert.Equal(expected, code.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Alpha2_Rejects_Empty_Input(string? input)
    {
        Assert.False(CountryAlpha2Code.TryParse(input, out _));
        Assert.Equal("country.alpha2.empty", CountryAlpha2Code.TryValidate(input, out _)!.Code);
    }

    [Theory]
    [InlineData("G")]
    [InlineData("GBR")]
    public void Alpha2_Rejects_Invalid_Length(string input)
    {
        Assert.False(CountryAlpha2Code.TryParse(input, out _));
        Assert.Equal("country.alpha2.invalid_length", CountryAlpha2Code.TryValidate(input, out _)!.Code);
    }

    [Theory]
    [InlineData("G1")]
    [InlineData("G-")]
    [InlineData("Gé")]
    public void Alpha2_Rejects_Non_Ascii_Letters(string input)
    {
        Assert.False(CountryAlpha2Code.TryParse(input, out _));
        Assert.Equal("country.alpha2.invalid_characters", CountryAlpha2Code.TryValidate(input, out _)!.Code);
    }

    [Theory]
    [InlineData("GBR", "GBR")]
    [InlineData("gbr", "GBR")]
    public void Alpha3_Parses_Uppercase_And_Lowercase(string input, string expected)
    {
        CountryAlpha3Code code = CountryAlpha3Code.Parse(input);

        Assert.Equal(expected, code.ToString());
    }

    [Theory]
    [InlineData(null, "country.alpha3.empty")]
    [InlineData("", "country.alpha3.empty")]
    [InlineData("GB", "country.alpha3.invalid_length")]
    [InlineData("GBR1", "country.alpha3.invalid_length")]
    [InlineData("GB1", "country.alpha3.invalid_characters")]
    public void Alpha3_Rejects_Invalid_Input(string? input, string expectedIssueCode)
    {
        Assert.False(CountryAlpha3Code.TryParse(input, out _));
        Assert.Equal(expectedIssueCode, CountryAlpha3Code.TryValidate(input, out _)!.Code);
    }

    [Theory]
    [InlineData("008")]
    [InlineData("036")]
    public void Numeric_Preserves_Leading_Zeroes(string input)
    {
        CountryNumericCode code = CountryNumericCode.Parse(input);

        Assert.Equal(input, code.ToString());
    }

    [Fact]
    public void Numeric_FromInt32_Preserves_Leading_Zeroes()
    {
        Assert.Equal("008", CountryNumericCode.FromInt32(8).ToString());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000)]
    public void Numeric_FromInt32_Rejects_Out_Of_Range_Values(int input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CountryNumericCode.FromInt32(input));
    }

    [Theory]
    [InlineData(null, "country.numeric.empty")]
    [InlineData("", "country.numeric.empty")]
    [InlineData("12", "country.numeric.invalid_length")]
    [InlineData("1234", "country.numeric.invalid_length")]
    [InlineData("12A", "country.numeric.invalid_characters")]
    public void Numeric_Rejects_Invalid_Input(string? input, string expectedIssueCode)
    {
        Assert.False(CountryNumericCode.TryParse(input, out _));
        Assert.Equal(expectedIssueCode, CountryNumericCode.TryValidate(input, out _)!.Code);
    }

    [Theory]
    [InlineData("GB-ENG", "GB", "ENG")]
    [InlineData("us-ca", "US", "CA")]
    [InlineData("IE-D", "IE", "D")]
    public void Subdivision_Parses_Representative_Valid_Codes(string input, string expectedCountry, string expectedPart)
    {
        CountrySubdivisionCode code = CountrySubdivisionCode.Parse(input);

        Assert.Equal(expectedCountry, code.CountryCode.ToString());
        Assert.Equal(expectedPart, code.SubdivisionPart);
        Assert.Equal(expectedCountry + "-" + expectedPart, code.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("GB")]
    [InlineData("GB-")]
    [InlineData("G-ENG")]
    [InlineData("GB-ENGL")]
    [InlineData("GB-E!")]
    public void Subdivision_Rejects_Invalid_Formats(string? input)
    {
        Assert.False(CountrySubdivisionCode.TryParse(input, out _));
    }

    [Fact]
    public void Default_Value_Objects_Are_Empty_Not_Valid_Canonical_Codes()
    {
        Assert.Equal(string.Empty, default(CountryAlpha2Code).Value);
        Assert.Equal(string.Empty, default(CountryAlpha3Code).Value);
        Assert.Equal(string.Empty, default(CountryNumericCode).Value);
        Assert.Equal(string.Empty, default(CountrySubdivisionCode).Value);
    }

    [Fact]
    public void Value_Objects_Use_Ordinal_Equality_And_Comparison()
    {
        Assert.Equal(CountryAlpha2Code.Parse("gb"), CountryAlpha2Code.Parse("GB"));
        Assert.True(CountryAlpha2Code.Parse("GB") < CountryAlpha2Code.Parse("US"));
        Assert.True(CountryAlpha3Code.Parse("GBR").CompareTo(CountryAlpha3Code.Parse("USA")) < 0);
        Assert.True(CountryNumericCode.Parse("008").CompareTo(CountryNumericCode.Parse("826")) < 0);
    }
}
