namespace ISOCodex.Countries;

/// <summary>
/// Represents the result of looking up mixed country-code input.
/// </summary>
public sealed class CountryCodeLookupResult
{
    private CountryCodeLookupResult(
        bool success,
        CountryInfo? country,
        CountryCodeKind? detectedKind,
        CountryCodeLookupFailureReason? failureReason,
        string? normalizedInput)
    {
        Success = success;
        Country = country;
        DetectedKind = detectedKind;
        FailureReason = failureReason;
        NormalizedInput = normalizedInput;
    }

    public bool Success { get; }

    public CountryInfo? Country { get; }

    public CountryCodeKind? DetectedKind { get; }

    public CountryCodeLookupFailureReason? FailureReason { get; }

    public string? NormalizedInput { get; }

    internal static CountryCodeLookupResult Found(CountryInfo country, CountryCodeKind kind, string normalizedInput) =>
        new(true, country, kind, null, normalizedInput);

    internal static CountryCodeLookupResult Failed(
        CountryCodeLookupFailureReason reason,
        CountryCodeKind? kind = null,
        string? normalizedInput = null) =>
        new(false, null, kind, reason, normalizedInput);
}
