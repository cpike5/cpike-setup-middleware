using cpike.Setup.Middleware.Models;
using cpike.Setup.Middleware.Steps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Default implementation of ISetupWizardService that manages wizard flow and navigation.
/// </summary>
public class SetupWizardService : ISetupWizardService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISetupStateManager _stateManager;
    private readonly ISetupCompletionService _completionService;
    private readonly ILogger<SetupWizardService> _logger;
    private readonly List<StepRegistration> _stepRegistrations;
    private int _currentStepIndex;

    public SetupWizardService(
        IServiceProvider serviceProvider,
        ISetupStateManager stateManager,
        ISetupCompletionService completionService,
        ILogger<SetupWizardService> logger,
        IEnumerable<StepRegistration> stepRegistrations)
    {
        _serviceProvider = serviceProvider;
        _stateManager = stateManager;
        _completionService = completionService;
        _logger = logger;
        _stepRegistrations = stepRegistrations.OrderBy(s => s.Order).ToList();
        _currentStepIndex = 0;

        _logger.LogInformation("SetupWizardService initialized with {StepCount} steps", _stepRegistrations.Count);
    }

    public int CurrentStepIndex => _currentStepIndex;

    public int TotalSteps => _stepRegistrations.Count;

    public bool CanNavigateNext => _currentStepIndex < _stepRegistrations.Count - 1;

    public bool CanNavigatePrevious => _currentStepIndex > 0;

    public int ProgressPercentage
    {
        get
        {
            if (_stepRegistrations.Count == 0) return 0;
            return (int)Math.Round((_currentStepIndex + 1) * 100.0 / _stepRegistrations.Count);
        }
    }

    public bool IsLastStep => _currentStepIndex == _stepRegistrations.Count - 1;

    public async Task<ISetupStep?> GetCurrentStepAsync()
    {
        if (_stepRegistrations.Count == 0)
        {
            _logger.LogWarning("No setup steps registered");
            return null;
        }

        if (_currentStepIndex < 0 || _currentStepIndex >= _stepRegistrations.Count)
        {
            _logger.LogError("Current step index {Index} is out of range", _currentStepIndex);
            return null;
        }

        var registration = _stepRegistrations[_currentStepIndex];
        var step = (ISetupStep)_serviceProvider.GetRequiredService(registration.StepType);

        _logger.LogDebug("Retrieved step {StepIndex}: {StepType}", _currentStepIndex, registration.StepType.Name);

        return await Task.FromResult(step);
    }

    public async Task<IEnumerable<ISetupStep>> GetAllStepsAsync()
    {
        var steps = new List<ISetupStep>();

        foreach (var registration in _stepRegistrations)
        {
            var step = (ISetupStep)_serviceProvider.GetRequiredService(registration.StepType);
            steps.Add(step);
        }

        _logger.LogDebug("Retrieved all {Count} steps", steps.Count);
        return await Task.FromResult(steps);
    }

    public async Task<bool> NextStepAsync()
    {
        if (!CanNavigateNext)
        {
            _logger.LogWarning("Cannot navigate next: already on last step");
            return false;
        }

        // Validate current step before proceeding
        var validationResult = await ValidateCurrentStepAsync();
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Cannot navigate next: current step validation failed with {ErrorCount} errors",
                validationResult.Errors.Count);
            return false;
        }

        // Execute OnNavigatingFrom lifecycle hook
        var currentStep = await GetCurrentStepAsync();
        if (currentStep != null)
        {
            try
            {
                await currentStep.OnNavigatingFromAsync();
                _logger.LogDebug("Executed OnNavigatingFromAsync for step {Index}", _currentStepIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingFromAsync for step {Index}", _currentStepIndex);
                return false;
            }
        }

        // Navigate to next step
        _currentStepIndex++;
        _logger.LogInformation("Navigated to step {Index} of {Total}", _currentStepIndex + 1, TotalSteps);

        // Execute OnNavigatingTo lifecycle hook
        var nextStep = await GetCurrentStepAsync();
        if (nextStep != null)
        {
            try
            {
                await nextStep.OnNavigatingToAsync();
                _logger.LogDebug("Executed OnNavigatingToAsync for step {Index}", _currentStepIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingToAsync for step {Index}", _currentStepIndex);
                // Don't return false here - we've already navigated
            }
        }

        return true;
    }

    public async Task<bool> PreviousStepAsync()
    {
        if (!CanNavigatePrevious)
        {
            _logger.LogWarning("Cannot navigate previous: already on first step");
            return false;
        }

        // Execute OnNavigatingFrom lifecycle hook
        var currentStep = await GetCurrentStepAsync();
        if (currentStep != null)
        {
            try
            {
                await currentStep.OnNavigatingFromAsync();
                _logger.LogDebug("Executed OnNavigatingFromAsync for step {Index}", _currentStepIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingFromAsync for step {Index}", _currentStepIndex);
                // Continue with navigation even if hook fails
            }
        }

        // Navigate to previous step
        _currentStepIndex--;
        _logger.LogInformation("Navigated back to step {Index} of {Total}", _currentStepIndex + 1, TotalSteps);

        // Execute OnNavigatingTo lifecycle hook
        var previousStep = await GetCurrentStepAsync();
        if (previousStep != null)
        {
            try
            {
                await previousStep.OnNavigatingToAsync();
                _logger.LogDebug("Executed OnNavigatingToAsync for step {Index}", _currentStepIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingToAsync for step {Index}", _currentStepIndex);
            }
        }

        return true;
    }

    public async Task<bool> NavigateToStepAsync(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= _stepRegistrations.Count)
        {
            _logger.LogWarning("Cannot navigate to step {Index}: index out of range", stepIndex);
            return false;
        }

        if (stepIndex == _currentStepIndex)
        {
            _logger.LogDebug("Already on step {Index}", stepIndex);
            return true;
        }

        // Execute OnNavigatingFrom for current step
        var currentStep = await GetCurrentStepAsync();
        if (currentStep != null)
        {
            try
            {
                await currentStep.OnNavigatingFromAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingFromAsync for step {Index}", _currentStepIndex);
            }
        }

        // Navigate
        var previousIndex = _currentStepIndex;
        _currentStepIndex = stepIndex;
        _logger.LogInformation("Navigated from step {PreviousIndex} to step {CurrentIndex}",
            previousIndex + 1, _currentStepIndex + 1);

        // Execute OnNavigatingTo for new step
        var newStep = await GetCurrentStepAsync();
        if (newStep != null)
        {
            try
            {
                await newStep.OnNavigatingToAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingToAsync for step {Index}", _currentStepIndex);
            }
        }

        return true;
    }

    public async Task<bool> CompleteSetupAsync()
    {
        _logger.LogInformation("Starting setup completion process");

        try
        {
            // Validate all steps
            var allSteps = await GetAllStepsAsync();
            foreach (var step in allSteps)
            {
                var validationResult = await step.ValidateAsync();
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Setup completion failed: step {StepTitle} validation failed", step.Title);
                    return false;
                }
            }

            // Execute all steps
            foreach (var step in allSteps)
            {
                try
                {
                    _logger.LogDebug("Executing step: {StepTitle}", step.Title);
                    await step.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Setup completion failed: error executing step {StepTitle}", step.Title);
                    return false;
                }
            }

            // Mark setup as complete
            await _completionService.MarkSetupCompleteAsync();
            _logger.LogInformation("Setup completed successfully");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Setup completion failed with unexpected error");
            return false;
        }
    }

    public async Task ResetWizardAsync()
    {
        _logger.LogInformation("Resetting wizard to first step");
        _currentStepIndex = 0;

        // Clear state
        _stateManager.Clear();

        // Execute OnNavigatingTo for first step
        var firstStep = await GetCurrentStepAsync();
        if (firstStep != null)
        {
            try
            {
                await firstStep.OnNavigatingToAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnNavigatingToAsync after reset");
            }
        }
    }

    public async Task<ValidationResult> ValidateCurrentStepAsync()
    {
        var currentStep = await GetCurrentStepAsync();
        if (currentStep == null)
        {
            _logger.LogWarning("Cannot validate: no current step");
            return ValidationResult.Failure("No current step available");
        }

        var result = await currentStep.ValidateAsync();
        _logger.LogDebug("Validated step {StepTitle}: {IsValid}", currentStep.Title, result.IsValid);

        return result;
    }
}
