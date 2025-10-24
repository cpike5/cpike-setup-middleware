namespace cpike.Setup.Middleware.Configuration;

/// <summary>
/// Configuration options for the setup wizard middleware.
/// </summary>
public class SetupOptions
{
    /// <summary>
    /// Gets the configuration section name for binding from appsettings.json.
    /// </summary>
    public const string SectionName = "Setup";

    /// <summary>
    /// Gets or sets the path where the setup wizard is accessible.
    /// Default is "/setup".
    /// </summary>
    public string SetupPath { get; set; } = "/setup";

    /// <summary>
    /// Gets or sets the directory where the setup completion marker file is stored.
    /// Default is "./App_Data".
    /// </summary>
    public string MarkerDirectory { get; set; } = "./App_Data";

    /// <summary>
    /// Gets or sets the name of the setup completion marker file.
    /// Default is ".setup-complete".
    /// </summary>
    public string MarkerFileName { get; set; } = ".setup-complete";

    /// <summary>
    /// Gets or sets the application version. This is stored in the completion marker.
    /// Default is "1.0.0".
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets a value indicating whether the setup wizard should be accessible
    /// only once. When true, deleting the marker file is required to re-run setup.
    /// Default is false (setup can be re-run if marker is deleted).
    /// </summary>
    public bool AllowSetupRerun { get; set; } = false;

    /// <summary>
    /// Gets or sets paths that should be excluded from setup redirection.
    /// These paths will be accessible even when setup is not complete.
    /// Common examples: "/health", "/api/status", "/_blazor", "/_framework"
    /// </summary>
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/_blazor",
        "/_framework",
        "/_content"
    };

    /// <summary>
    /// Gets or sets a value indicating whether password protection is required
    /// to access the setup wizard. Default is true.
    /// When enabled, a secure password will be generated and logged to the console.
    /// </summary>
    public bool RequirePassword { get; set; } = true;
}
