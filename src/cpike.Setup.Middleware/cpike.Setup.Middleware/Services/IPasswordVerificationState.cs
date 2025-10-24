namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Service for managing password verification state across requests.
/// This is designed to work with Blazor Server's component lifecycle.
/// </summary>
public interface IPasswordVerificationState
{
    /// <summary>
    /// Marks the password as verified for the current client.
    /// </summary>
    /// <param name="clientId">The unique identifier for the client.</param>
    void MarkPasswordVerified(string clientId);

    /// <summary>
    /// Checks if the password has been verified for the current client.
    /// </summary>
    /// <param name="clientId">The unique identifier for the client.</param>
    /// <returns>True if the password has been verified, false otherwise.</returns>
    bool IsPasswordVerified(string clientId);

    /// <summary>
    /// Clears the verification state for the current client.
    /// </summary>
    /// <param name="clientId">The unique identifier for the client.</param>
    void ClearVerification(string clientId);
}
