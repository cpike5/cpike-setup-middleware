using cpike.Setup.Middleware.Configuration;
using cpike.Setup.Middleware.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cpike.Setup.Middleware;

/// <summary>
/// Middleware that intercepts HTTP requests and redirects to the setup wizard
/// when the application setup has not been completed.
/// </summary>
public class SetupMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SetupOptions _options;
    private readonly ILogger<SetupMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetupMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="options">The setup configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public SetupMiddleware(
        RequestDelegate next,
        IOptions<SetupOptions> options,
        ILogger<SetupMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to check setup completion and redirect if necessary.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="completionService">The setup completion service (injected per-request).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context, ISetupCompletionService completionService)
    {
        // Quick exit: Check if setup is complete (most common case in production)
        if (await completionService.IsSetupCompleteAsync())
        {
            await _next(context);
            return;
        }

        var requestPath = context.Request.Path.Value ?? string.Empty;

        // Allow access to excluded paths (Blazor framework files, health checks, etc.)
        if (IsPathExcluded(requestPath))
        {
            await _next(context);
            return;
        }

        // Allow access to setup wizard itself
        if (requestPath.StartsWith(_options.SetupPath, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Setup not complete and not accessing setup wizard - redirect
        _logger.LogDebug("Setup not complete. Redirecting {Path} to {SetupPath}",
            requestPath, _options.SetupPath);

        context.Response.Redirect(_options.SetupPath);
    }

    /// <summary>
    /// Checks if a request path should be excluded from setup redirection.
    /// </summary>
    /// <param name="path">The request path to check.</param>
    /// <returns>True if the path should be excluded; otherwise, false.</returns>
    private bool IsPathExcluded(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        foreach (var excludedPath in _options.ExcludedPaths)
        {
            if (path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
