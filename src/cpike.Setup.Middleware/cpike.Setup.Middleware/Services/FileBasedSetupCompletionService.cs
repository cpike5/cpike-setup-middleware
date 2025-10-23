using System.Text.Json;
using cpike.Setup.Middleware.Configuration;
using cpike.Setup.Middleware.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// File-based implementation of <see cref="ISetupCompletionService"/> that uses a marker file
/// to track setup completion status.
/// </summary>
public class FileBasedSetupCompletionService : ISetupCompletionService
{
    private readonly SetupOptions _options;
    private readonly ILogger<FileBasedSetupCompletionService> _logger;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="FileBasedSetupCompletionService"/> class.
    /// </summary>
    /// <param name="options">The setup configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public FileBasedSetupCompletionService(
        IOptions<SetupOptions> options,
        ILogger<FileBasedSetupCompletionService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> IsSetupCompleteAsync()
    {
        try
        {
            var markerPath = GetMarkerFilePath();

            if (!File.Exists(markerPath))
            {
                _logger.LogDebug("Setup completion marker not found at {MarkerPath}", markerPath);
                return false;
            }

            // Verify the marker file is valid
            await _fileLock.WaitAsync();
            try
            {
                var content = await File.ReadAllTextAsync(markerPath);
                var marker = JsonSerializer.Deserialize<SetupCompletionMarker>(content);

                if (marker == null)
                {
                    _logger.LogWarning("Setup completion marker at {MarkerPath} is invalid or corrupted", markerPath);
                    return false;
                }

                _logger.LogDebug("Setup was completed at {CompletedAt} (Version: {Version})",
                    marker.CompletedAtUtc, marker.Version);
                return true;
            }
            finally
            {
                _fileLock.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking setup completion status");
            // Fail open - if we can't read the marker, assume setup is not complete
            return false;
        }
    }

    /// <inheritdoc />
    public async Task MarkSetupCompleteAsync()
    {
        try
        {
            var markerPath = GetMarkerFilePath();
            var markerDirectory = Path.GetDirectoryName(markerPath);

            // Ensure the directory exists
            if (!string.IsNullOrEmpty(markerDirectory) && !Directory.Exists(markerDirectory))
            {
                Directory.CreateDirectory(markerDirectory);
                _logger.LogInformation("Created marker directory at {Directory}", markerDirectory);
            }

            var marker = new SetupCompletionMarker
            {
                CompletedAtUtc = DateTime.UtcNow,
                Version = _options.Version,
                MiddlewareVersion = "1.0.0"
            };

            await _fileLock.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(marker, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(markerPath, json);

                _logger.LogInformation("Setup marked as complete. Marker file created at {MarkerPath}", markerPath);
            }
            finally
            {
                _fileLock.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark setup as complete");
            throw new InvalidOperationException(
                "Failed to create setup completion marker. Ensure the application has write permissions to the marker directory.",
                ex);
        }
    }

    /// <inheritdoc />
    public async Task ClearSetupCompletionAsync()
    {
        try
        {
            var markerPath = GetMarkerFilePath();

            if (!File.Exists(markerPath))
            {
                _logger.LogDebug("Setup completion marker does not exist at {MarkerPath}, nothing to clear", markerPath);
                return;
            }

            await _fileLock.WaitAsync();
            try
            {
                File.Delete(markerPath);
                _logger.LogInformation("Setup completion marker cleared at {MarkerPath}", markerPath);
            }
            finally
            {
                _fileLock.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear setup completion marker");
            throw new InvalidOperationException(
                "Failed to delete setup completion marker. Ensure the application has delete permissions.",
                ex);
        }
    }

    /// <summary>
    /// Gets the full path to the marker file.
    /// </summary>
    /// <returns>The marker file path.</returns>
    private string GetMarkerFilePath()
    {
        var directory = string.IsNullOrWhiteSpace(_options.MarkerDirectory)
            ? Path.Combine(Directory.GetCurrentDirectory(), "App_Data")
            : _options.MarkerDirectory;

        var fileName = string.IsNullOrWhiteSpace(_options.MarkerFileName)
            ? ".setup-complete"
            : _options.MarkerFileName;

        return Path.Combine(directory, fileName);
    }
}
