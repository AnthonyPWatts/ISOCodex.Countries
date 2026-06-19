namespace ISOCodex.Countries;

/// <summary>
/// Describes the status of a country entry in the registry.
/// </summary>
public enum CountryEntryStatus
{
    /// <summary>
    /// A current country or territory entry in the package data snapshot.
    /// </summary>
    Current = 0,

    /// <summary>
    /// A former entry, if future package data explicitly models former country codes.
    /// </summary>
    Former = 1,

    /// <summary>
    /// A reserved entry, if future package data explicitly models reserved code ranges.
    /// </summary>
    Reserved = 2,

    /// <summary>
    /// An exceptionally reserved entry, if future package data explicitly models exceptional reservations.
    /// </summary>
    ExceptionallyReserved = 3,

    /// <summary>
    /// A transitionally reserved entry, if future package data explicitly models transitional reservations.
    /// </summary>
    TransitionallyReserved = 4,

    /// <summary>
    /// A user-assigned entry, if future package data explicitly models user-assigned code ranges.
    /// </summary>
    UserAssigned = 5,

    /// <summary>
    /// A status that is not known from the package data snapshot.
    /// </summary>
    Unknown = 99
}
