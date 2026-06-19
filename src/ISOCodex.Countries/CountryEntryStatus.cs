namespace ISOCodex.Countries;

/// <summary>
/// Describes the status of a country entry in the registry.
/// </summary>
public enum CountryEntryStatus
{
    Current = 0,
    Former = 1,
    Reserved = 2,
    ExceptionallyReserved = 3,
    TransitionallyReserved = 4,
    UserAssigned = 5,
    Unknown = 99
}
