namespace ISOCodex.Countries;

/// <summary>
/// Describes why a country-code lookup failed.
/// </summary>
public enum CountryCodeLookupFailureReason
{
    Empty,
    InvalidSyntax,
    Ambiguous,
    Unknown,
    ReservedButNotCountry,
    Unsupported
}
