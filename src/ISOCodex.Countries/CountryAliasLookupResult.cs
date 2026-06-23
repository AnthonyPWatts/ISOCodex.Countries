namespace ISOCodex.Countries;

/// <summary>
/// Represents the result of explicit country alias lookup.
/// </summary>
public sealed class CountryAliasLookupResult
{
    private CountryAliasLookupResult(
        bool success,
        CountryInfo? country,
        string? matchedAlias,
        bool ambiguous,
        IReadOnlyList<CountryInfo> candidates,
        CountryCodeLookupFailureReason? failureReason)
    {
        Success = success;
        Country = country;
        MatchedAlias = matchedAlias;
        Ambiguous = ambiguous;
        Candidates = candidates;
        FailureReason = failureReason;
    }

    public bool Success { get; }

    public CountryInfo? Country { get; }

    public string? MatchedAlias { get; }

    public bool Ambiguous { get; }

    public IReadOnlyList<CountryInfo> Candidates { get; }

    public CountryCodeLookupFailureReason? FailureReason { get; }

    internal static CountryAliasLookupResult Found(CountryInfo country, string matchedAlias) =>
        new(true, country, matchedAlias, false, new[] { country }, null);

    internal static CountryAliasLookupResult AmbiguousMatch(string matchedAlias, IReadOnlyList<CountryInfo> candidates) =>
        new(false, null, matchedAlias, true, candidates, CountryCodeLookupFailureReason.Ambiguous);

    internal static CountryAliasLookupResult Failed(CountryCodeLookupFailureReason reason) =>
        new(false, null, null, false, Array.Empty<CountryInfo>(), reason);
}
