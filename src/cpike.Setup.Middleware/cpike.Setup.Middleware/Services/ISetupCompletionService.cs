namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Service responsible for tracking whether the setup wizard has been completed.
/// </summary>
public interface ISetupCompletionService
{
    /// <summary>
    /// Checks if the setup process has been completed.
    /// </summary>
    /// <returns>True if setup is complete, false otherwise.</returns>
    Task<bool> IsSetupCompleteAsync();

    /// <summary>
    /// Marks the setup process as complete and persists the completion state.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkSetupCompleteAsync();

    /// <summary>
    /// Clears the setup completion marker, allowing the wizard to run again.
    /// This is typically used for development or re-configuration scenarios.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearSetupCompletionAsync();
}
