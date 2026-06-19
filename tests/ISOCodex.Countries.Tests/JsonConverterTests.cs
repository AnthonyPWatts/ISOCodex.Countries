using System.Text.Json;

namespace ISOCodex.Countries.Tests;

public sealed class JsonConverterTests
{
    private static JsonSerializerOptions Options
    {
        get
        {
            return CountryJsonSerializerOptions.CreateDefault();
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
    public void Adds_Converters_To_Existing_Options()
    {
        JsonSerializerOptions options = new();

        CountryJsonSerializerOptions.AddConverters(options);

        Assert.Equal("\"GB\"", JsonSerializer.Serialize(CountryAlpha2Code.Parse("GB"), options));
    }
}
