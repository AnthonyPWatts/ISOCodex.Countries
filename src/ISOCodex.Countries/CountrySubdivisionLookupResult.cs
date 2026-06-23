namespace ISOCodex.Countries;

/// <summary>
/// Represents the result of looking up a country subdivision code.
/// </summary>
public sealed class CountrySubdivisionLookupResult
{
    private CountrySubdivisionLookupResult(
        bool success,
        CountrySubdivisionInfo? subdivision,
        CountrySubdivisionCode? code,
        CountryCodeLookupFailureReason? failureReason,
        string? normalizedInput)
    {
        Success = success;
        Subdivision = subdivision;
        Code = code;
        FailureReason = failureReason;
        NormalizedInput = normalizedInput;
    }

    public bool Success { get; }

    public CountrySubdivisionInfo? Subdivision { get; }

    public CountrySubdivisionCode? Code { get; }

    public CountryCodeLookupFailureReason? FailureReason { get; }

    public string? NormalizedInput { get; }

    internal static CountrySubdivisionLookupResult Found(CountrySubdivisionInfo subdivision, CountrySubdivisionCode code) =>
        new(true, subdivision, code, null, code.Value);

    internal static CountrySubdivisionLookupResult Failed(
        CountryCodeLookupFailureReason reason,
        CountrySubdivisionCode? code = null,
        string? normalizedInput = null) =>
        new(false, null, code, reason, normalizedInput);
}
