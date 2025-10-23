namespace cpike.Setup.Middleware.Models;

/// <summary>
/// Represents the result of a validation operation on a setup step.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the collection of validation error messages.
    /// Empty if validation was successful.
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    /// <param name="isValid">Whether the validation was successful.</param>
    /// <param name="errors">The collection of error messages.</param>
    private ValidationResult(bool isValid, IEnumerable<string> errors)
    {
        IsValid = isValid;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    }

    /// <summary>
    /// Gets a successful validation result with no errors.
    /// </summary>
    public static ValidationResult Success => new(true, Enumerable.Empty<string>());

    /// <summary>
    /// Creates a failed validation result with a single error message.
    /// </summary>
    /// <param name="errorMessage">The validation error message.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failure(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty.", nameof(errorMessage));

        return new ValidationResult(false, new[] { errorMessage });
    }

    /// <summary>
    /// Creates a failed validation result with multiple error messages.
    /// </summary>
    /// <param name="errorMessages">The collection of validation error messages.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failure(IEnumerable<string> errorMessages)
    {
        var errors = errorMessages?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList()
            ?? throw new ArgumentNullException(nameof(errorMessages));

        if (errors.Count == 0)
            throw new ArgumentException("At least one error message must be provided.", nameof(errorMessages));

        return new ValidationResult(false, errors);
    }

    /// <summary>
    /// Creates a failed validation result with multiple error messages.
    /// </summary>
    /// <param name="errorMessages">The collection of validation error messages.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failure(params string[] errorMessages)
    {
        return Failure(errorMessages.AsEnumerable());
    }
}
