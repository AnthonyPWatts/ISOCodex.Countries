namespace ISOCodex.Countries;

/// <summary>
/// Describes why a country-code lookup failed.
/// </summary>
public enum CountryCodeLookupFailureReason
{
    /// <summary>
    /// The input was null, empty, or whitespace.
    /// </summary>
    Empty,

    /// <summary>
    /// The input did not match any supported country-code syntax.
    /// </summary>
    InvalidSyntax,

    /// <summary>
    /// The input could not be resolved because more than one meaning is possible.
    /// </summary>
    Ambiguous,

    /// <summary>
    /// The input has valid syntax but is not present in the current package data snapshot.
    /// </summary>
    Unknown,

    /// <summary>
    /// The input is reserved or special-purpose rather than a country, if future data explicitly supports that distinction.
    /// </summary>
    ReservedButNotCountry,

    /// <summary>
    /// The input uses a code shape or model that the current package does not support.
    /// </summary>
    Unsupported
}
