namespace ISOCodex.Countries;

/// <summary>
/// Describes the kind of a country-code-shaped element.
/// </summary>
public enum CountryCodeElementKind
{
    CurrentCountry = 0,
    RegionGrouping = 1,
    PseudoTerritory = 2,
    UnknownRegion = 3,
    UserAssigned = 4,
    DeprecatedAlias = 5,
    SpecialPurpose = 6,
    Other = 99
}
