namespace ISOCodex.Countries;

/// <summary>
/// Describes the checked-in data snapshot exposed by the package.
/// </summary>
public static class CountryDataVersion
{
    /// <summary>
    /// Gets the package data snapshot identifier.
    /// </summary>
    public static string Identifier { get; } = "cldr-48.2-country-subdivision-seed-2026-06";

    /// <summary>
    /// Gets the date this data posture was checked.
    /// </summary>
    public static string CheckedOn { get; } = "2026-06-19";

    /// <summary>
    /// Gets a human-readable description of the package data coverage.
    /// </summary>
    public static string Description { get; } =
        "Current ISO-style country, territory, and regular subdivision seed data derived from Unicode CLDR 48.2.";
}
