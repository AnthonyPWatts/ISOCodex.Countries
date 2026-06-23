namespace ISOCodex.Countries;

/// <summary>
/// Describes the source shape of a country display name.
/// </summary>
public enum CountryDisplayNameKind
{
    /// <summary>
    /// A short display name.
    /// </summary>
    Short = 0,

    /// <summary>
    /// An official-form display name when the source distinguishes one.
    /// </summary>
    Official = 1,

    /// <summary>
    /// A variant display name when the source distinguishes one.
    /// </summary>
    Variant = 2
}
