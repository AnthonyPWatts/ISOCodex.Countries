using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISOCodex.Countries.Tests;

public sealed class DataSeedDriftTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public void Country_Json_Seed_Mirrors_Compiled_Registry_Data()
    {
        CountrySeedRecord[] jsonCountries = ReadSeedFile<CountrySeedRecord>("countries.seed.json");

        var compiled = CountryRegistry.All
            .Select(country => new CountrySeedRecord(
                country.Alpha2.Value,
                country.Alpha3.Value,
                country.Numeric.Value,
                country.EnglishShortName,
                country.EnglishOfficialName,
                country.Status,
                country.CommonAliases.ToArray(),
                country.Notes.ToArray()))
            .ToArray();

        Assert.Equal(FormatCountries(compiled), FormatCountries(jsonCountries));
    }

    [Fact]
    public void Subdivision_Json_Seed_Mirrors_Compiled_Registry_Data()
    {
        SubdivisionSeedRecord[] jsonSubdivisions = ReadSeedFile<SubdivisionSeedRecord>("subdivisions.seed.json");

        var compiled = CountrySubdivisionRegistry.All
            .Select(subdivision => new SubdivisionSeedRecord(
                subdivision.Code.Value,
                subdivision.CountryCode.Value,
                subdivision.EnglishName,
                subdivision.LocalName,
                subdivision.Type))
            .ToArray();

        Assert.Equal(compiled, jsonSubdivisions);
    }

    [Fact]
    public void Country_Display_Name_Json_Seed_Mirrors_Compiled_Registry_Data()
    {
        CountryDisplayNameSeedRecord[] jsonNames = ReadSeedFile<CountryDisplayNameSeedRecord>("country-names.seed.json");

        var compiled = CountryNameRegistry.All
            .Select(name => new CountryDisplayNameSeedRecord(
                name.CountryCode.Value,
                name.LanguageTag,
                name.Name,
                name.Kind,
                name.IsEndonym,
                name.IsRightToLeft))
            .ToArray();

        Assert.Equal(compiled, jsonNames);
    }

    [Fact]
    public void Country_Alias_Json_Seed_Mirrors_Compiled_Registry_Data()
    {
        CountryAliasSeedRecord[] jsonAliases = ReadSeedFile<CountryAliasSeedRecord>("country-aliases.seed.json");

        var compiled = CountryAliasRegistry.All
            .Select(alias => new CountryAliasSeedRecord(
                alias.Alias,
                alias.ReplacementCountryCode?.Value,
                alias.Kind,
                alias.Source,
                alias.Notes))
            .ToArray();

        Assert.Equal(compiled, jsonAliases);
    }

    [Fact]
    public void Country_Code_Element_Json_Seed_Mirrors_Compiled_Registry_Data()
    {
        CountryCodeElementSeedRecord[] jsonElements = ReadSeedFile<CountryCodeElementSeedRecord>("country-code-elements.seed.json");

        var compiled = CountryCodeElementRegistry.All
            .Select(element => new CountryCodeElementSeedRecord(
                element.Alpha2.Value,
                element.Kind,
                element.DisplayName,
                element.Source,
                element.Notes))
            .ToArray();

        Assert.Equal(compiled, jsonElements);
    }

    [Fact]
    public void Json_Seed_Files_Are_Valid_Json()
    {
        Assert.NotEmpty(ReadSeedFile<CountrySeedRecord>("countries.seed.json"));
        Assert.NotEmpty(ReadSeedFile<SubdivisionSeedRecord>("subdivisions.seed.json"));
        Assert.NotEmpty(ReadSeedFile<CountryDisplayNameSeedRecord>("country-names.seed.json"));
        Assert.NotEmpty(ReadSeedFile<CountryAliasSeedRecord>("country-aliases.seed.json"));
        Assert.NotEmpty(ReadSeedFile<CountryCodeElementSeedRecord>("country-code-elements.seed.json"));
    }

    private static T[] ReadSeedFile<T>(string fileName)
    {
        string path = Path.Combine(FindRepositoryRoot(), "data", fileName);
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T[]>(json, JsonOptions) ?? Array.Empty<T>();
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ISOCodex.Countries.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root.");
    }

    private static string[] FormatCountries(IEnumerable<CountrySeedRecord> countries) =>
        countries
            .Select(country => string.Join(
                "|",
                country.Alpha2,
                country.Alpha3,
                country.Numeric,
                country.EnglishShortName,
                country.EnglishOfficialName ?? string.Empty,
                country.Status.ToString(),
                string.Join(",", country.CommonAliases ?? Array.Empty<string>()),
                string.Join(",", country.Notes ?? Array.Empty<string>())))
            .ToArray();

    private sealed record CountrySeedRecord(
        string Alpha2,
        string Alpha3,
        string Numeric,
        string EnglishShortName,
        string? EnglishOfficialName,
        CountryEntryStatus Status,
        string[]? CommonAliases = null,
        string[]? Notes = null);

    private sealed record SubdivisionSeedRecord(
        string Code,
        string CountryCode,
        string EnglishName,
        string? LocalName,
        CountrySubdivisionType Type);

    private sealed record CountryDisplayNameSeedRecord(
        string CountryCode,
        string LanguageTag,
        string Name,
        CountryDisplayNameKind Kind,
        bool IsEndonym,
        bool IsRightToLeft);

    private sealed record CountryAliasSeedRecord(
        string Alias,
        string? ReplacementCountryCode,
        CountryAliasKind Kind,
        string Source,
        string? Notes);

    private sealed record CountryCodeElementSeedRecord(
        string Alpha2,
        CountryCodeElementKind Kind,
        string DisplayName,
        string Source,
        string? Notes);
}
