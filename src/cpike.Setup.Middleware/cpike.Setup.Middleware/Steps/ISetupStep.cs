using cpike.Setup.Middleware.Models;

namespace cpike.Setup.Middleware.Steps;

/// <summary>
/// Defines the contract for a setup step in the wizard.
/// </summary>
public interface ISetupStep
{
    /// <summary>
    /// Gets the title of this setup step, displayed to the user.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the description of this setup step, providing context to the user.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the execution order of this step. Steps with lower order values execute first.
    /// Default is 0.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Validates the data entered in this step before proceeding to the next step.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResult"/> indicating success or containing error messages.
    /// </returns>
    Task<ValidationResult> ValidateAsync();

    /// <summary>
    /// Executes the business logic for this step (e.g., saving data, configuring services).
    /// This is called after validation succeeds when navigating to the next step.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync();

    /// <summary>
    /// Called when the wizard is navigating to this step.
    /// Use this to initialize data or prepare the step for display.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnNavigatingToAsync();

    /// <summary>
    /// Called when the wizard is navigating away from this step.
    /// Use this to cleanup resources or perform final actions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnNavigatingFromAsync();
}
