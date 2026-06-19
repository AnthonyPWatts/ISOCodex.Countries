namespace ISOCodex.Countries;

internal static class CountryCodeSyntax
{
    public static CountryCodeValidationIssue? ValidateAlphaCode(
        string? value,
        int expectedLength,
        string issuePrefix,
        string displayName,
        out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return new CountryCodeValidationIssue(issuePrefix + ".empty", displayName + " is required.", value);
        }

        if (value.Length != expectedLength)
        {
            return new CountryCodeValidationIssue(
                issuePrefix + ".invalid_length",
                displayName + " must be exactly " + expectedLength.ToString(System.Globalization.CultureInfo.InvariantCulture) + " characters.",
                value);
        }

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            bool isAsciiLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

            if (!isAsciiLetter)
            {
                return new CountryCodeValidationIssue(issuePrefix + ".invalid_characters", displayName + " must contain ASCII letters only.", value);
            }
        }

        normalized = value.ToUpperInvariant();
        return null;
    }

    public static CountryCodeValidationIssue? ValidateNumericCode(string? value, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return new CountryCodeValidationIssue("country.numeric.empty", "Country numeric code is required.", value);
        }

        if (value.Length != 3)
        {
            return new CountryCodeValidationIssue("country.numeric.invalid_length", "Country numeric code must be exactly 3 digits.", value);
        }

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];

            if (c < '0' || c > '9')
            {
                return new CountryCodeValidationIssue("country.numeric.invalid_characters", "Country numeric code must contain digits only.", value);
            }
        }

        normalized = value;
        return null;
    }
}
