using System.Text.Json;

namespace ISOCodex.Countries;

/// <summary>
/// Provides helper methods for registering ISOCodex country-code JSON converters.
/// </summary>
public static class CountryJsonSerializerOptions
{
    /// <summary>
    /// Creates serializer options with all ISOCodex country-code converters registered.
    /// </summary>
    public static JsonSerializerOptions CreateDefault()
    {
        JsonSerializerOptions options = new();
        AddConverters(options);
        return options;
    }

    /// <summary>
    /// Adds all ISOCodex country-code converters to existing serializer options.
    /// </summary>
    public static void AddConverters(JsonSerializerOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Converters.Add(new CountryAlpha2CodeJsonConverter());
        options.Converters.Add(new CountryAlpha3CodeJsonConverter());
        options.Converters.Add(new CountryNumericCodeJsonConverter());
        options.Converters.Add(new CountrySubdivisionCodeJsonConverter());
    }
}
