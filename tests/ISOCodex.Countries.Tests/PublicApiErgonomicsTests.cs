using System.Text.Json;

namespace ISOCodex.Countries.Tests;

public sealed class PublicApiErgonomicsTests
{
    [Fact]
    public void Consumer_Can_Parse_Alpha2_Then_Use_Registry_Lookup()
    {
        CountryAlpha2Code code = CountryAlpha2Code.Parse("gb");

        CountryInfo country = CountryRegistry.GetByAlpha2(code);

        Assert.Equal("GB", country.Alpha2.Value);
        Assert.Equal("United Kingdom", country.EnglishShortName);
    }

    [Fact]
    public void Consumer_Can_TryGet_From_Untrusted_Input()
    {
        Assert.True(CountryRegistry.TryGetByAlpha2("gb", out CountryInfo? country));

        Assert.Equal("United Kingdom", country?.EnglishShortName);
    }

    [Theory]
    [InlineData("GB", CountryCodeKind.Alpha2)]
    [InlineData("GBR", CountryCodeKind.Alpha3)]
    [InlineData("826", CountryCodeKind.Numeric)]
    public void Consumer_Can_Use_Mixed_Lookup_For_Successful_Input(string input, CountryCodeKind expectedKind)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.True(result.Success);
        Assert.Equal(expectedKind, result.DetectedKind);
        Assert.Equal("United Kingdom", result.Country?.EnglishShortName);
    }

    [Theory]
    [InlineData("AA", CountryCodeLookupFailureReason.Unknown)]
    [InlineData("ZZ", CountryCodeLookupFailureReason.ReservedButNotCountry)]
    [InlineData("12!", CountryCodeLookupFailureReason.InvalidSyntax)]
    public void Consumer_Can_Use_Mixed_Lookup_For_Failed_Input(string input, CountryCodeLookupFailureReason expectedReason)
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup(input);

        Assert.False(result.Success);
        Assert.Equal(expectedReason, result.FailureReason);
    }

    [Fact]
    public void Uk_Does_Not_Resolve_Silently_To_Gb()
    {
        CountryCodeLookupResult result = CountryRegistry.Lookup("UK");

        Assert.False(result.Success);
        Assert.Equal(CountryCodeLookupFailureReason.Unknown, result.FailureReason);
    }

    [Fact]
    public void Consumer_Dto_Can_Round_Trip_Json_With_Manual_Converters()
    {
        JsonSerializerOptions options = CreateOptions();
        ConsumerCountryDto dto = new(CountryAlpha2Code.Parse("gb"), CountryNumericCode.Parse("008"));

        string json = JsonSerializer.Serialize(dto, options);
        ConsumerCountryDto? deserialized = JsonSerializer.Deserialize<ConsumerCountryDto>(json, options);

        Assert.NotNull(deserialized);
        Assert.Equal("GB", deserialized.CountryCode.ToString());
        Assert.Equal("008", deserialized.NumericCode.ToString());
    }

    [Fact]
    public void Consumer_Can_Build_Country_Dropdown_Options()
    {
        var options = CountryRegistry.All
            .OrderBy(country => country.EnglishShortName, StringComparer.Ordinal)
            .Select(country => new { Value = country.Alpha2.Value, Label = country.EnglishShortName })
            .ToArray();

        Assert.Contains(options, option => option.Value == "GB" && option.Label == "United Kingdom");
    }

    [Fact]
    public void Consumer_Can_Look_Up_Representative_Subdivision()
    {
        CountrySubdivisionCode code = CountrySubdivisionCode.Parse("gb-eng");

        CountrySubdivisionInfo subdivision = CountrySubdivisionRegistry.GetByCode(code);

        Assert.Equal("England", subdivision.EnglishName);
        Assert.Equal("GB", subdivision.CountryCode.Value);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new CountryAlpha2CodeJsonConverter());
        options.Converters.Add(new CountryAlpha3CodeJsonConverter());
        options.Converters.Add(new CountryNumericCodeJsonConverter());
        options.Converters.Add(new CountrySubdivisionCodeJsonConverter());
        return options;
    }

    private sealed record ConsumerCountryDto(CountryAlpha2Code CountryCode, CountryNumericCode NumericCode);
}
