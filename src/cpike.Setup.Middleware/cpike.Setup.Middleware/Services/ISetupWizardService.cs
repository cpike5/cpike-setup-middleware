using cpike.Setup.Middleware.Steps;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Manages the setup wizard flow, navigation, and state orchestration.
/// </summary>
public interface ISetupWizardService
{
    /// <summary>
    /// Gets the current step index (zero-based).
    /// </summary>
    int CurrentStepIndex { get; }

    /// <summary>
    /// Gets the total number of registered steps.
    /// </summary>
    int TotalSteps { get; }

    /// <summary>
    /// Gets whether navigation to the next step is allowed.
    /// </summary>
    bool CanNavigateNext { get; }

    /// <summary>
    /// Gets whether navigation to the previous step is allowed.
    /// </summary>
    bool CanNavigatePrevious { get; }

    /// <summary>
    /// Gets the progress percentage (0-100).
    /// </summary>
    int ProgressPercentage { get; }

    /// <summary>
    /// Gets whether the wizard is on the last step.
    /// </summary>
    bool IsLastStep { get; }

    /// <summary>
    /// Gets the current step instance.
    /// </summary>
    /// <returns>The current setup step, or null if no steps are registered.</returns>
    Task<ISetupStep?> GetCurrentStepAsync();

    /// <summary>
    /// Gets all registered setup steps in order.
    /// </summary>
    /// <returns>Collection of all setup steps.</returns>
    Task<IEnumerable<ISetupStep>> GetAllStepsAsync();

    /// <summary>
    /// Navigates to the next step after validating the current step.
    /// </summary>
    /// <returns>True if navigation succeeded, false if validation failed or already on last step.</returns>
    Task<bool> NextStepAsync();

    /// <summary>
    /// Navigates to the previous step.
    /// </summary>
    /// <returns>True if navigation succeeded, false if already on first step.</returns>
    Task<bool> PreviousStepAsync();

    /// <summary>
    /// Navigates to a specific step by index.
    /// </summary>
    /// <param name="stepIndex">Zero-based step index to navigate to.</param>
    /// <returns>True if navigation succeeded, false if index is out of range.</returns>
    Task<bool> NavigateToStepAsync(int stepIndex);

    /// <summary>
    /// Completes the setup wizard by executing all steps and marking setup as complete.
    /// </summary>
    /// <returns>True if setup completed successfully, false if any step execution failed.</returns>
    Task<bool> CompleteSetupAsync();

    /// <summary>
    /// Resets the wizard to the first step.
    /// </summary>
    Task ResetWizardAsync();

    /// <summary>
    /// Gets the validation result for the current step without navigating.
    /// </summary>
    /// <returns>Validation result for the current step.</returns>
    Task<Models.ValidationResult> ValidateCurrentStepAsync();
}
