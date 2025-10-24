using cpike.Setup.Middleware.Configuration;
using cpike.Setup.Middleware.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Default implementation of ISetupPasswordService that manages password protection for the setup wizard.
/// </summary>
public class SetupPasswordService : ISetupPasswordService
{
    private const string PasswordSessionKey = "SetupPasswordVerified";
    private const string PasswordFileName = ".setup-password";
    private const int MaxAttempts = 5;
    private const int LockoutSeconds = 60;
    private const int PasswordLength = 16; // 4 segments of 4 characters

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SetupPasswordService> _logger;
    private readonly SetupOptions _options;
    private readonly string _passwordFilePath;
    private readonly Dictionary<string, AttemptsInfo> _attemptTracker = new();
    private readonly object _lockObject = new();

    public SetupPasswordService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<SetupPasswordService> logger,
        IOptions<SetupOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _options = options.Value;
        _passwordFilePath = Path.Combine(_options.MarkerDirectory, PasswordFileName);
    }

    public async Task<string> GeneratePasswordAsync()
    {
        var password = GenerateSecurePassword();

        // Ensure directory exists
        Directory.CreateDirectory(_options.MarkerDirectory);

        // Store password hash (not plaintext) in file
        var passwordHash = HashPassword(password);
        await File.WriteAllTextAsync(_passwordFilePath, passwordHash);

        // Log to console with formatting
        LogPasswordToConsole(password);

        _logger.LogInformation("Setup password generated and saved to {FilePath}", _passwordFilePath);

        return password;
    }

    public async Task<PasswordValidationResult> ValidatePasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordValidationResult.Failure("Password is required.", MaxAttempts);
        }

        // Check rate limiting
        var clientId = GetClientIdentifier();
        var attemptInfo = GetAttemptInfo(clientId);

        if (attemptInfo.IsLockedOut)
        {
            var secondsRemaining = (int)(attemptInfo.LockoutUntil!.Value - DateTime.UtcNow).TotalSeconds;
            if (secondsRemaining > 0)
            {
                _logger.LogWarning("Client {ClientId} is locked out for {Seconds} more seconds",
                    clientId, secondsRemaining);
                return PasswordValidationResult.Lockout(secondsRemaining);
            }
            else
            {
                // Lockout expired, reset
                ResetAttempts(clientId);
                attemptInfo = GetAttemptInfo(clientId);
            }
        }

        // Check if password file exists
        if (!File.Exists(_passwordFilePath))
        {
            _logger.LogWarning("Password file not found at {FilePath}", _passwordFilePath);
            return PasswordValidationResult.Failure("Setup password not configured.", MaxAttempts);
        }

        // Read and verify password
        var storedHash = await File.ReadAllTextAsync(_passwordFilePath);
        var providedHash = HashPassword(password);

        if (storedHash.Trim() == providedHash.Trim())
        {
            // Success - reset attempts
            ResetAttempts(clientId);
            await MarkPasswordVerifiedAsync();
            _logger.LogInformation("Setup password verified successfully for client {ClientId}", clientId);
            return PasswordValidationResult.Success();
        }

        // Failed attempt - increment counter
        attemptInfo.Attempts++;
        attemptInfo.LastAttempt = DateTime.UtcNow;

        var remainingAttempts = MaxAttempts - attemptInfo.Attempts;

        if (attemptInfo.Attempts >= MaxAttempts)
        {
            // Lock out the client
            attemptInfo.LockoutUntil = DateTime.UtcNow.AddSeconds(LockoutSeconds);
            _logger.LogWarning("Client {ClientId} locked out after {Attempts} failed attempts",
                clientId, attemptInfo.Attempts);
            return PasswordValidationResult.Lockout(LockoutSeconds);
        }

        _logger.LogWarning("Invalid password attempt for client {ClientId}. {Remaining} attempts remaining",
            clientId, remainingAttempts);
        return PasswordValidationResult.Failure(
            $"Invalid password. {remainingAttempts} attempt(s) remaining.",
            remainingAttempts);
    }

    public async Task<bool> IsPasswordRequiredAsync()
    {
        if (!_options.RequirePassword)
        {
            return false;
        }

        // Check if password file exists
        if (!File.Exists(_passwordFilePath))
        {
            // Generate password on first access
            await GeneratePasswordAsync();
            return true;
        }

        return true;
    }

    public Task MarkPasswordVerifiedAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            session.SetString(PasswordSessionKey, "true");
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsPasswordVerifiedAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            var verified = session.GetString(PasswordSessionKey);
            return Task.FromResult(verified == "true");
        }

        return Task.FromResult(false);
    }

    public async Task DeletePasswordAsync()
    {
        if (File.Exists(_passwordFilePath))
        {
            File.Delete(_passwordFilePath);
            _logger.LogInformation("Setup password file deleted");
        }

        await Task.CompletedTask;
    }

    private string GenerateSecurePassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluded ambiguous characters
        var segments = new string[4];

        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            var segment = new char[4];
            for (int j = 0; j < 4; j++)
            {
                rng.GetBytes(buffer);
                var randomIndex = BitConverter.ToUInt32(buffer, 0) % chars.Length;
                segment[j] = chars[(int)randomIndex];
            }
            segments[i] = new string(segment);
        }

        return string.Join("-", segments);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private void LogPasswordToConsole(string password)
    {
        var border = new string('=', 60);
        var message = new StringBuilder();
        message.AppendLine();
        message.AppendLine(border);
        message.AppendLine("*** SETUP WIZARD PASSWORD ***");
        message.AppendLine(border);
        message.AppendLine();
        message.AppendLine($"  Password: {password}");
        message.AppendLine();
        message.AppendLine("  This password is required to access the setup wizard.");
        message.AppendLine("  It will be deleted automatically after setup is complete.");
        message.AppendLine();
        message.AppendLine(border);

        Console.WriteLine(message.ToString());
    }

    private string GetClientIdentifier()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Use IP address as client identifier
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
        return "unknown";
    }

    private AttemptsInfo GetAttemptInfo(string clientId)
    {
        lock (_lockObject)
        {
            if (!_attemptTracker.TryGetValue(clientId, out var info))
            {
                info = new AttemptsInfo();
                _attemptTracker[clientId] = info;
            }
            return info;
        }
    }

    private void ResetAttempts(string clientId)
    {
        lock (_lockObject)
        {
            if (_attemptTracker.ContainsKey(clientId))
            {
                _attemptTracker.Remove(clientId);
            }
        }
    }

    private class AttemptsInfo
    {
        public int Attempts { get; set; }
        public DateTime? LastAttempt { get; set; }
        public DateTime? LockoutUntil { get; set; }

        public bool IsLockedOut => LockoutUntil.HasValue && LockoutUntil.Value > DateTime.UtcNow;
    }
}
