namespace cpike.Setup.Middleware.Models;

/// <summary>
/// Represents a registered setup step with its configuration.
/// </summary>
public class StepRegistration
{
    /// <summary>
    /// Gets the type of the setup step.
    /// </summary>
    public Type StepType { get; }

    /// <summary>
    /// Gets the order in which this step should execute.
    /// Lower numbers execute first. Default is 0.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepRegistration"/> class.
    /// </summary>
    /// <param name="stepType">The type of the setup step.</param>
    /// <param name="order">The execution order. Default is 0.</param>
    public StepRegistration(Type stepType, int order = 0)
    {
        StepType = stepType ?? throw new ArgumentNullException(nameof(stepType));
        Order = order;
    }
}
