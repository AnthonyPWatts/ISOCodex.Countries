namespace ISOCodex.Countries;

/// <summary>
/// Provides lookup access to generated country display names.
/// </summary>
public static class CountryNameRegistry
{
    public static IReadOnlyList<CountryDisplayName> All { get; } = CountrySeedData.CountryDisplayNames;

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<CountryDisplayName>> ByCountry =
        All.GroupBy(name => name.CountryCode.Value, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<CountryDisplayName>)group
                    .OrderBy(name => name.LanguageTag, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(name => name.Kind)
                    .ToArray(),
                StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, CountryDisplayName> ByCountryLanguageAndKind =
        All.ToDictionary(
            name => BuildKey(name.CountryCode, name.LanguageTag, name.Kind),
            StringComparer.OrdinalIgnoreCase);

    public static string GetEnglishShortName(CountryAlpha2Code code) =>
        TryGetEnglishShortName(code, out string? name)
            ? name!
            : throw new KeyNotFoundException("No country display name is known for alpha-2 code '" + code + "'.");

    public static bool TryGetEnglishShortName(CountryAlpha2Code code, out string? name)
    {
        name = null;

        if (TryFind(code, "en", CountryDisplayNameKind.Short, out CountryDisplayName? displayName))
        {
            name = displayName!.Name;
            return true;
        }

        if (CountryRegistry.TryGetByAlpha2(code, out CountryInfo? country))
        {
            name = country!.EnglishShortName;
            return true;
        }

        return false;
    }

    public static CountryDisplayNameLookupResult LookupDisplayName(CountryAlpha2Code code, string languageTag)
    {
        string requestedLanguageTag = NormalizeLanguageTag(languageTag);

        if (requestedLanguageTag.Length == 0)
        {
            return CountryDisplayNameLookupResult.Failed(requestedLanguageTag, CountryCodeLookupFailureReason.Empty);
        }

        if (!CountryRegistry.TryGetByAlpha2(code, out CountryInfo? country))
        {
            return CountryDisplayNameLookupResult.Failed(requestedLanguageTag, CountryCodeLookupFailureReason.Unknown);
        }

        if (TryFind(code, requestedLanguageTag, CountryDisplayNameKind.Short, out CountryDisplayName? displayName))
        {
            return CountryDisplayNameLookupResult.Found(displayName!, requestedLanguageTag, usedFallback: false);
        }

        string? parentTag = GetParentLanguageTag(requestedLanguageTag);
        if (parentTag is not null && TryFind(code, parentTag, CountryDisplayNameKind.Short, out displayName))
        {
            return CountryDisplayNameLookupResult.Found(displayName!, requestedLanguageTag, usedFallback: true);
        }

        if (TryFind(code, "en", CountryDisplayNameKind.Short, out displayName))
        {
            return CountryDisplayNameLookupResult.Found(displayName!, requestedLanguageTag, usedFallback: true);
        }

        CountryDisplayName fallback = new(
            code,
            "en",
            country!.EnglishShortName,
            CountryDisplayNameKind.Short,
            isEndonym: false,
            isRightToLeft: false);

        return CountryDisplayNameLookupResult.Found(fallback, requestedLanguageTag, usedFallback: true);
    }

    public static bool TryGetDisplayName(
        CountryAlpha2Code code,
        string languageTag,
        out CountryDisplayName? displayName)
    {
        CountryDisplayNameLookupResult result = LookupDisplayName(code, languageTag);
        displayName = result.DisplayName;
        return result.Success;
    }

    public static IReadOnlyList<CountryDisplayName> GetDisplayNames(CountryAlpha2Code code) =>
        ByCountry.TryGetValue(code.Value, out IReadOnlyList<CountryDisplayName>? names)
            ? names
            : Array.Empty<CountryDisplayName>();

    public static IReadOnlyList<CountryDisplayName> GetEndonyms(CountryAlpha2Code code) =>
        GetDisplayNames(code)
            .Where(name => name.IsEndonym)
            .OrderBy(name => name.LanguageTag, StringComparer.OrdinalIgnoreCase)
            .ThenBy(name => name.Name, StringComparer.Ordinal)
            .ToArray();

    public static bool TryGetPrimaryEndonym(CountryAlpha2Code code, out CountryDisplayName? displayName)
    {
        displayName = GetEndonyms(code).FirstOrDefault();
        return displayName is not null;
    }

    private static bool TryFind(
        CountryAlpha2Code code,
        string languageTag,
        CountryDisplayNameKind kind,
        out CountryDisplayName? displayName) =>
        ByCountryLanguageAndKind.TryGetValue(BuildKey(code, languageTag, kind), out displayName);

    private static string BuildKey(CountryAlpha2Code code, string languageTag, CountryDisplayNameKind kind) =>
        code.Value + "|" + NormalizeLanguageTag(languageTag) + "|" + (int)kind;

    private static string NormalizeLanguageTag(string? languageTag) =>
        (languageTag ?? string.Empty).Trim().Replace('_', '-');

    private static string? GetParentLanguageTag(string languageTag)
    {
        int index = languageTag.LastIndexOf('-');
        return index > 0 ? languageTag[..index] : null;
    }
}
