using System.Text.Json;

namespace ISOCodex.Countries.Tests;

public sealed class JsonConverterTests
{
    private static JsonSerializerOptions Options
    {
        get
        {
            JsonSerializerOptions options = new();
            options.Converters.Add(new CountryAlpha2CodeJsonConverter());
            options.Converters.Add(new CountryAlpha3CodeJsonConverter());
            options.Converters.Add(new CountryNumericCodeJsonConverter());
            options.Converters.Add(new CountrySubdivisionCodeJsonConverter());
            return options;
        }
    }

    [Fact]
    public void Serialises_Canonical_Codes()
    {
        Assert.Equal("\"GB\"", JsonSerializer.Serialize(CountryAlpha2Code.Parse("gb"), Options));
        Assert.Equal("\"GBR\"", JsonSerializer.Serialize(CountryAlpha3Code.Parse("gbr"), Options));
        Assert.Equal("\"008\"", JsonSerializer.Serialize(CountryNumericCode.Parse("008"), Options));
        Assert.Equal("\"GB-ENG\"", JsonSerializer.Serialize(CountrySubdivisionCode.Parse("gb-eng"), Options));
    }

    [Fact]
    public void Deserialises_Valid_Codes()
    {
        Assert.Equal("GB", JsonSerializer.Deserialize<CountryAlpha2Code>("\"gb\"", Options).ToString());
        Assert.Equal("GBR", JsonSerializer.Deserialize<CountryAlpha3Code>("\"gbr\"", Options).ToString());
        Assert.Equal("008", JsonSerializer.Deserialize<CountryNumericCode>("\"008\"", Options).ToString());
        Assert.Equal("GB-ENG", JsonSerializer.Deserialize<CountrySubdivisionCode>("\"gb-eng\"", Options).ToString());
    }

    [Theory]
    [InlineData("\"G1\"")]
    [InlineData("null")]
    public void Rejects_Invalid_Codes(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CountryAlpha2Code>(json, Options));
    }

    [Fact]
    public void Dto_Round_Trip_Preserves_Canonical_Strings()
    {
        CountryDto dto = new(
            CountryAlpha2Code.Parse("gb"),
            CountryAlpha3Code.Parse("gbr"),
            CountryNumericCode.Parse("008"),
            CountrySubdivisionCode.Parse("gb-eng"));

        string json = JsonSerializer.Serialize(dto, Options);
        CountryDto? deserialized = JsonSerializer.Deserialize<CountryDto>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Equal("GB", deserialized.Alpha2.ToString());
        Assert.Equal("GBR", deserialized.Alpha3.ToString());
        Assert.Equal("008", deserialized.Numeric.ToString());
        Assert.Equal("GB-ENG", deserialized.Subdivision.ToString());
    }

    [Fact]
    public void Nullable_Code_Properties_Can_Round_Trip_Null()
    {
        NullableCountryDto dto = new(null);

        string json = JsonSerializer.Serialize(dto, Options);
        NullableCountryDto? deserialized = JsonSerializer.Deserialize<NullableCountryDto>(json, Options);

        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Alpha2);
    }

    [Theory]
    [InlineData("\"G1\"", typeof(CountryAlpha2Code))]
    [InlineData("\"GB1\"", typeof(CountryAlpha3Code))]
    [InlineData("\"08A\"", typeof(CountryNumericCode))]
    [InlineData("\"GB-!\"", typeof(CountrySubdivisionCode))]
    public void Each_Converter_Rejects_Invalid_Values(string json, Type targetType)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize(json, targetType, Options));
    }

    private sealed record CountryDto(
        CountryAlpha2Code Alpha2,
        CountryAlpha3Code Alpha3,
        CountryNumericCode Numeric,
        CountrySubdivisionCode Subdivision);

    private sealed record NullableCountryDto(CountryAlpha2Code? Alpha2);
}
