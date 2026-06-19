namespace ISOCodex.Countries;

/// <summary>
/// Describes the checked-in data snapshot exposed by the package.
/// </summary>
public static class CountryDataVersion
{
    /// <summary>
    /// Gets the package data snapshot identifier.
    /// </summary>
    public static string Identifier { get; } = "representative-seed-2026-06";

    /// <summary>
    /// Gets the date this representative seed posture was checked.
    /// </summary>
    public static string CheckedOn { get; } = "2026-06-19";

    /// <summary>
    /// Gets a human-readable description of the package data coverage.
    /// </summary>
    public static string Description { get; } =
        "Representative country and subdivision seed data for API and package validation; not complete ISO 3166 coverage.";
}
