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
    /// The input could not be resolved deterministically.
    /// </summary>
    Ambiguous,

    /// <summary>
    /// The input had valid syntax but is not in the current seed registry.
    /// </summary>
    Unknown,

    /// <summary>
    /// The input is reserved, special-use, user-assigned, or otherwise not a package country entry.
    /// </summary>
    ReservedButNotCountry,

    /// <summary>
    /// The input shape is recognised but not supported by this package version.
    /// </summary>
    Unsupported
}
