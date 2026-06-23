using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISOCodex.Countries;

/// <summary>
/// Serialises alpha-2 country codes as canonical strings.
/// </summary>
public sealed class CountryAlpha2CodeJsonConverter : JsonConverter<CountryAlpha2Code>
{
    public override CountryAlpha2Code Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        JsonCodeParser.Parse(ref reader, CountryAlpha2Code.Parse, "country alpha-2 code");

    public override void Write(Utf8JsonWriter writer, CountryAlpha2Code value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

/// <summary>
/// Serialises alpha-3 country codes as canonical strings.
/// </summary>
public sealed class CountryAlpha3CodeJsonConverter : JsonConverter<CountryAlpha3Code>
{
    public override CountryAlpha3Code Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        JsonCodeParser.Parse(ref reader, CountryAlpha3Code.Parse, "country alpha-3 code");

    public override void Write(Utf8JsonWriter writer, CountryAlpha3Code value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

/// <summary>
/// Serialises country numeric codes as canonical strings.
/// </summary>
public sealed class CountryNumericCodeJsonConverter : JsonConverter<CountryNumericCode>
{
    public override CountryNumericCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        JsonCodeParser.Parse(ref reader, CountryNumericCode.Parse, "country numeric code");

    public override void Write(Utf8JsonWriter writer, CountryNumericCode value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

/// <summary>
/// Serialises country subdivision codes as canonical strings.
/// </summary>
public sealed class CountrySubdivisionCodeJsonConverter : JsonConverter<CountrySubdivisionCode>
{
    public override CountrySubdivisionCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        JsonCodeParser.Parse(ref reader, CountrySubdivisionCode.Parse, "country subdivision code");

    public override void Write(Utf8JsonWriter writer, CountrySubdivisionCode value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

internal static class JsonCodeParser
{
    public static T Parse<T>(ref Utf8JsonReader reader, Func<string, T> parser, string displayName)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected a string " + displayName + ".");
        }

        string? value = reader.GetString();

        if (value is null)
        {
            throw new JsonException("Expected a string " + displayName + ".");
        }

        try
        {
            return parser(value);
        }
        catch (FormatException ex)
        {
            throw new JsonException("Invalid " + displayName + ": " + ex.Message, ex);
        }
    }
}
