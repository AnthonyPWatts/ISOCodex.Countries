namespace ISOCodex.Countries;

/// <summary>
/// Represents the result of looking up a country display name.
/// </summary>
public sealed class CountryDisplayNameLookupResult
{
    private CountryDisplayNameLookupResult(
        bool success,
        CountryDisplayName? displayName,
        string requestedLanguageTag,
        string? resolvedLanguageTag,
        bool usedFallback,
        CountryCodeLookupFailureReason? failureReason)
    {
        Success = success;
        DisplayName = displayName;
        RequestedLanguageTag = requestedLanguageTag;
        ResolvedLanguageTag = resolvedLanguageTag;
        UsedFallback = usedFallback;
        FailureReason = failureReason;
    }

    public bool Success { get; }

    public CountryDisplayName? DisplayName { get; }

    public string RequestedLanguageTag { get; }

    public string? ResolvedLanguageTag { get; }

    public bool UsedFallback { get; }

    public CountryCodeLookupFailureReason? FailureReason { get; }

    internal static CountryDisplayNameLookupResult Found(
        CountryDisplayName displayName,
        string requestedLanguageTag,
        bool usedFallback) =>
        new(true, displayName, requestedLanguageTag, displayName.LanguageTag, usedFallback, null);

    internal static CountryDisplayNameLookupResult Failed(
        string requestedLanguageTag,
        CountryCodeLookupFailureReason reason) =>
        new(false, null, requestedLanguageTag, null, false, reason);
}
