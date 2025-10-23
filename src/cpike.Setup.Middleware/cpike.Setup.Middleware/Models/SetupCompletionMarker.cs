namespace cpike.Setup.Middleware.Models;

/// <summary>
/// Represents the data stored in the setup completion marker file.
/// </summary>
public class SetupCompletionMarker
{
    /// <summary>
    /// Gets or sets the UTC timestamp when setup was completed.
    /// </summary>
    public DateTime CompletedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the version of the application when setup was completed.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the setup middleware library.
    /// </summary>
    public string MiddlewareVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets optional metadata about the setup completion.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
