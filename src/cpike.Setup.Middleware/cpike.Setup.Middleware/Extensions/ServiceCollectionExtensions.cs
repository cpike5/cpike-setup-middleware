using cpike.Setup.Middleware.Configuration;
using cpike.Setup.Middleware.Services;
using cpike.Setup.Middleware.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace cpike.Setup.Middleware.Extensions;

/// <summary>
/// Extension methods for configuring setup wizard services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the setup wizard services and configures setup steps using a fluent builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration callback for registering steps.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddSetupWizard(setup =>
    /// {
    ///     setup.AddStep&lt;AdminAccountStep&gt;();
    ///     setup.AddStep&lt;DatabaseConfigStep&gt;(order: 20);
    ///     setup.WithOptions(o => o.SetupPath = "/setup");
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddSetupWizard(
        this IServiceCollection services,
        Action<SetupBuilder> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        // Register core services
        services.TryAddSingleton<ISetupCompletionService, FileBasedSetupCompletionService>();
        services.TryAddScoped<ISetupStateManager, SetupStateManager>();
        services.TryAddScoped<ISetupWizardService, SetupWizardService>();
        services.TryAddSingleton<ISetupPasswordService, SetupPasswordService>();
        services.TryAddSingleton<IPasswordVerificationState, PasswordVerificationStateService>();

        // Register HttpContextAccessor (required for password service client identification)
        services.AddHttpContextAccessor();

        // Configure setup options with default values
        services.AddOptions<SetupOptions>()
            .BindConfiguration(SetupOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Build and validate setup configuration
        var builder = new SetupBuilder(services);
        configure(builder);
        builder.Validate();

        // Store step registrations for wizard service to consume
        var steps = builder.GetSteps();
        services.AddSingleton<IEnumerable<Models.StepRegistration>>(_ => steps);

        return services;
    }

    /// <summary>
    /// Adds the setup wizard services with configuration binding from appsettings.json.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="configure">The configuration callback for registering steps.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSetupWizard(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SetupBuilder> configure)
    {
        services.Configure<SetupOptions>(
            configuration.GetSection(SetupOptions.SectionName));

        return services.AddSetupWizard(configure);
    }
}
