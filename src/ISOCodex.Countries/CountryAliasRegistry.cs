using System.Text;

namespace ISOCodex.Countries;

/// <summary>
/// Provides explicit opt-in country alias lookup.
/// </summary>
public static class CountryAliasRegistry
{
    public static IReadOnlyList<CountryAliasInfo> All { get; } = CountrySeedData.CountryAliases;

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<CountryAliasInfo>> ByAlias =
        All.GroupBy(alias => NormalizeAlias(alias.Alias), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<CountryAliasInfo>)group.ToArray(),
                StringComparer.OrdinalIgnoreCase);

    public static CountryAliasLookupResult Lookup(string? value)
    {
        string key = NormalizeAlias(value);

        if (key.Length == 0)
        {
            return CountryAliasLookupResult.Failed(CountryCodeLookupFailureReason.Empty);
        }

        if (!ByAlias.TryGetValue(key, out IReadOnlyList<CountryAliasInfo>? aliases))
        {
            return CountryAliasLookupResult.Failed(CountryCodeLookupFailureReason.Unknown);
        }

        CountryInfo[] candidates = aliases
            .Where(alias => alias.ReplacementCountryCode.HasValue)
            .Select(alias => alias.ReplacementCountryCode!.Value)
            .Distinct()
            .Select(code => CountryRegistry.TryGetByAlpha2(code, out CountryInfo? country) ? country : null)
            .Where(country => country is not null)
            .Cast<CountryInfo>()
            .OrderBy(country => country.Alpha2.Value, StringComparer.Ordinal)
            .ToArray();

        string matchedAlias = aliases[0].Alias;

        if (candidates.Length == 1)
        {
            return CountryAliasLookupResult.Found(candidates[0], matchedAlias);
        }

        if (candidates.Length > 1)
        {
            return CountryAliasLookupResult.AmbiguousMatch(matchedAlias, candidates);
        }

        return CountryAliasLookupResult.Failed(CountryCodeLookupFailureReason.Unknown);
    }

    public static bool TryGetByAlias(string? value, out CountryInfo? country)
    {
        CountryAliasLookupResult result = Lookup(value);
        country = result.Country;
        return result.Success;
    }

    private static string NormalizeAlias(string? alias) =>
        (alias ?? string.Empty).Trim().Normalize(NormalizationForm.FormC);
}
