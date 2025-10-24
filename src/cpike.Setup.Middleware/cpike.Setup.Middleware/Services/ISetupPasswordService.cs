using cpike.Setup.Middleware.Models;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Service for managing setup wizard password protection.
/// </summary>
public interface ISetupPasswordService
{
    /// <summary>
    /// Generates a new random password and stores it securely.
    /// The password is also logged to the console for the administrator to access.
    /// </summary>
    /// <returns>The generated password in format: XXXX-XXXX-XXXX-XXXX</returns>
    Task<string> GeneratePasswordAsync();

    /// <summary>
    /// Validates the provided password against the stored password.
    /// Implements rate limiting to prevent brute force attacks.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>A validation result indicating success or failure with rate limiting information.</returns>
    Task<PasswordValidationResult> ValidatePasswordAsync(string password);

    /// <summary>
    /// Checks if a password has been generated and is required.
    /// </summary>
    /// <returns>True if password protection is active and required.</returns>
    Task<bool> IsPasswordRequiredAsync();

    /// <summary>
    /// Marks the password as verified for the current session.
    /// </summary>
    Task MarkPasswordVerifiedAsync();

    /// <summary>
    /// Checks if the password has been verified for the current session.
    /// </summary>
    /// <returns>True if the password has been verified in the current session.</returns>
    Task<bool> IsPasswordVerifiedAsync();

    /// <summary>
    /// Deletes the password file after setup is complete.
    /// </summary>
    Task DeletePasswordAsync();
}
