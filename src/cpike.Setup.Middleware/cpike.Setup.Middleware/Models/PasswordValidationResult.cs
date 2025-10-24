namespace cpike.Setup.Middleware.Models;

/// <summary>
/// Represents the result of a password validation attempt.
/// </summary>
public class PasswordValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the password validation was successful.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the error message if validation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the number of remaining attempts before lockout.
    /// </summary>
    public int RemainingAttempts { get; }

    /// <summary>
    /// Gets the number of seconds until the next attempt is allowed (for rate limiting).
    /// </summary>
    public int SecondsUntilNextAttempt { get; }

    /// <summary>
    /// Gets a value indicating whether the user is currently locked out.
    /// </summary>
    public bool IsLockedOut => SecondsUntilNextAttempt > 0;

    private PasswordValidationResult(
        bool isValid,
        string? errorMessage = null,
        int remainingAttempts = 0,
        int secondsUntilNextAttempt = 0)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        RemainingAttempts = remainingAttempts;
        SecondsUntilNextAttempt = secondsUntilNextAttempt;
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static PasswordValidationResult Success()
    {
        return new PasswordValidationResult(true);
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errorMessage">The error message describing why validation failed.</param>
    /// <param name="remainingAttempts">The number of attempts remaining before lockout.</param>
    public static PasswordValidationResult Failure(string errorMessage, int remainingAttempts)
    {
        return new PasswordValidationResult(false, errorMessage, remainingAttempts);
    }

    /// <summary>
    /// Creates a lockout validation result.
    /// </summary>
    /// <param name="secondsUntilNextAttempt">The number of seconds until the next attempt is allowed.</param>
    public static PasswordValidationResult Lockout(int secondsUntilNextAttempt)
    {
        return new PasswordValidationResult(
            false,
            $"Too many failed attempts. Please wait {secondsUntilNextAttempt} seconds before trying again.",
            0,
            secondsUntilNextAttempt);
    }
}
