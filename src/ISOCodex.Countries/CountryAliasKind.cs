namespace ISOCodex.Countries;

/// <summary>
/// Describes the source shape of a country alias.
/// </summary>
public enum CountryAliasKind
{
    CommonName = 0,
    DeprecatedCode = 1,
    HistoricalCode = 2,
    DisplayName = 3,
    Other = 99
}
