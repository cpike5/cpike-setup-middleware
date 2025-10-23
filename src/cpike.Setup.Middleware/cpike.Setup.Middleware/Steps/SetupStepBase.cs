using cpike.Setup.Middleware.Models;
using cpike.Setup.Middleware.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace cpike.Setup.Middleware.Steps;

/// <summary>
/// Abstract base class for setup steps, providing common functionality and dependency injection.
/// Inherit from this class to create custom setup steps with minimal boilerplate.
/// </summary>
public abstract class SetupStepBase : ComponentBase, ISetupStep
{
    /// <summary>
    /// Gets the state manager for sharing data between steps.
    /// </summary>
    [Inject]
    protected ISetupStateManager StateManager { get; set; } = null!;

    /// <summary>
    /// Gets the navigation manager for programmatic navigation.
    /// </summary>
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;

    /// <summary>
    /// Gets the logger instance for this step.
    /// </summary>
    [Inject]
    protected ILogger<SetupStepBase> Logger { get; set; } = null!;

    /// <inheritdoc />
    public abstract string Title { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public virtual int Order => 0;

    /// <inheritdoc />
    /// <remarks>
    /// Override this method to provide custom validation logic.
    /// The default implementation returns <see cref="ValidationResult.Success"/>.
    /// </remarks>
    public virtual Task<ValidationResult> ValidateAsync()
    {
        return Task.FromResult(ValidationResult.Success);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Override this method to implement the business logic for this step.
    /// This method is called after validation succeeds, before navigating to the next step.
    /// </remarks>
    public abstract Task ExecuteAsync();

    /// <inheritdoc />
    /// <remarks>
    /// Override this method to perform initialization when navigating to this step.
    /// The default implementation does nothing.
    /// </remarks>
    public virtual Task OnNavigatingToAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Override this method to perform cleanup when navigating away from this step.
    /// The default implementation does nothing.
    /// </remarks>
    public virtual Task OnNavigatingFromAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Helper method to create a validation failure with a single error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed validation result.</returns>
    protected ValidationResult Invalid(string errorMessage)
    {
        return ValidationResult.Failure(errorMessage);
    }

    /// <summary>
    /// Helper method to create a validation failure with multiple error messages.
    /// </summary>
    /// <param name="errorMessages">The collection of error messages.</param>
    /// <returns>A failed validation result.</returns>
    protected ValidationResult Invalid(params string[] errorMessages)
    {
        return ValidationResult.Failure(errorMessages);
    }

    /// <summary>
    /// Helper method to create a successful validation result.
    /// </summary>
    /// <returns>A successful validation result.</returns>
    protected ValidationResult Valid()
    {
        return ValidationResult.Success;
    }
}
