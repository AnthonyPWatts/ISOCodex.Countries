namespace ISOCodex.Countries;

/// <summary>
/// Describes a structured country-code validation failure.
/// </summary>
public sealed class CountryCodeValidationIssue
{
    /// <summary>
    /// Creates a validation issue.
    /// </summary>
    public CountryCodeValidationIssue(string code, string message, string? input)
    {
        Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentException("Issue code is required.", nameof(code)) : code;
        Message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentException("Issue message is required.", nameof(message)) : message;
        Input = input;
    }

    /// <summary>
    /// Gets the stable machine-readable issue code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets a human-readable validation message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the input that failed validation, when available.
    /// </summary>
    public string? Input { get; }
}
