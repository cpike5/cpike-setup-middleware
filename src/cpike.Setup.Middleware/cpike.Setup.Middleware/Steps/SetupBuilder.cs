using cpike.Setup.Middleware.Configuration;
using cpike.Setup.Middleware.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cpike.Setup.Middleware.Steps;

/// <summary>
/// Builder for configuring the setup wizard and registering setup steps.
/// </summary>
public class SetupBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<StepRegistration> _steps = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SetupBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public SetupBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Adds a setup step to the wizard with the default order (0).
    /// </summary>
    /// <typeparam name="TStep">The type of the setup step.</typeparam>
    /// <returns>The builder instance for fluent configuration.</returns>
    public SetupBuilder AddStep<TStep>() where TStep : class
    {
        _services.AddScoped<TStep>();
        _steps.Add(new StepRegistration(typeof(TStep)));
        return this;
    }

    /// <summary>
    /// Adds a setup step to the wizard with a specific execution order.
    /// Steps with lower order values execute first.
    /// </summary>
    /// <typeparam name="TStep">The type of the setup step.</typeparam>
    /// <param name="order">The execution order for this step.</param>
    /// <returns>The builder instance for fluent configuration.</returns>
    public SetupBuilder AddStep<TStep>(int order) where TStep : class
    {
        _services.AddScoped<TStep>();
        _steps.Add(new StepRegistration(typeof(TStep), order));
        return this;
    }

    /// <summary>
    /// Configures setup options using a callback.
    /// </summary>
    /// <param name="configure">The configuration callback.</param>
    /// <returns>The builder instance for fluent configuration.</returns>
    public SetupBuilder WithOptions(Action<SetupOptions> configure)
    {
        _services.Configure(configure);
        return this;
    }

    /// <summary>
    /// Gets the list of registered steps.
    /// </summary>
    /// <returns>A read-only list of step registrations.</returns>
    internal IReadOnlyList<StepRegistration> GetSteps()
    {
        return _steps.AsReadOnly();
    }

    /// <summary>
    /// Validates the setup configuration. Called internally during service registration.
    /// </summary>
    internal void Validate()
    {
        if (_steps.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one setup step must be registered. Use AddStep<T>() to register steps.");
        }

        // Verify all step types are unique
        var duplicates = _steps
            .GroupBy(s => s.StepType)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key.Name)
            .ToList();

        if (duplicates.Any())
        {
            throw new InvalidOperationException(
                $"Duplicate step types registered: {string.Join(", ", duplicates)}. " +
                "Each step type can only be registered once.");
        }
    }
}
