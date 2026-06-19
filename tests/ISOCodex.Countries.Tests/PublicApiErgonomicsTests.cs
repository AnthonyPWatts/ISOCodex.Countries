using System.Text.Json;

namespace ISOCodex.Countries.Tests;

public sealed class PublicApiErgonomicsTests
{
    [Fact]
    public void Typical_Parse_And_Lookup_Flow_Is_Straightforward()
    {
        CountryAlpha2Code gb = CountryAlpha2Code.Parse("GB");
        CountryInfo country = CountryRegistry.GetByAlpha2(gb);

        Assert.Equal("United Kingdom", country.EnglishShortName);
        Assert.Equal("GBR", country.Alpha3.ToString());
    }

    [Fact]
    public void Typical_TryGet_Flow_Uses_Value_Object_When_Available()
    {
        CountryAlpha2Code code = CountryAlpha2Code.Parse("ie");

        bool found = CountryRegistry.TryGetByAlpha2(code, out CountryInfo? country);

        Assert.True(found);
        Assert.Equal("Ireland", country?.EnglishShortName);
    }

    [Fact]
    public void String_TryGet_Flow_Is_Useful_At_Boundaries()
    {
        string? input = "008";

        bool found = CountryRegistry.TryGetByNumeric(input, out CountryInfo? country);

        Assert.True(found);
        Assert.Equal("AL", country?.Alpha2.ToString());
    }

    [Fact]
    public void Mixed_Lookup_Failure_Flow_Is_Structured()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("UK");

        Assert.False(result.Success);
        Assert.Null(result.Country);
        Assert.Equal(CountryCodeKind.Alpha2, result.DetectedKind);
        Assert.Equal(CountryCodeLookupFailureReason.ReservedButNotCountry, result.FailureReason);
        Assert.Equal("UK", result.NormalizedInput);
    }

    [Fact]
    public void Json_Dto_Flow_Uses_Registration_Helper()
    {
        JsonSerializerOptions options = CountryJsonSerializerOptions.CreateDefault();
        CustomerDto? dto = JsonSerializer.Deserialize<CustomerDto>("{\"Country\":\"gb\",\"Subdivision\":\"gb-eng\"}", options);

        Assert.NotNull(dto);
        Assert.Equal("GB", dto.Country.ToString());
        Assert.Equal("GB-ENG", dto.Subdivision.ToString());
    }

    [Fact]
    public void Country_Dropdown_Flow_Uses_Canonical_Alpha2_Values()
    {
        var options = CountryRegistry.All
            .OrderBy(country => country.EnglishShortName)
            .Select(country => new CountryOption(country.Alpha2.Value, country.EnglishShortName))
            .ToArray();

        Assert.Contains(options, option => option.Value == "GB" && option.Label == "United Kingdom");
        Assert.DoesNotContain(options, option => option.Value == "UK");
    }

    private sealed record CustomerDto(CountryAlpha2Code Country, CountrySubdivisionCode Subdivision);

    private sealed record CountryOption(string Value, string Label);
}
