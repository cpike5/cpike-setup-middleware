using System.Collections.Concurrent;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// In-memory implementation of password verification state management.
/// Uses a thread-safe concurrent dictionary to track verified clients.
/// </summary>
public class PasswordVerificationStateService : IPasswordVerificationState
{
    private readonly ConcurrentDictionary<string, VerificationInfo> _verifiedClients = new();
    private const int ExpirationMinutes = 60; // Verification expires after 1 hour

    public void MarkPasswordVerified(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return;
        }

        _verifiedClients[clientId] = new VerificationInfo
        {
            VerifiedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(ExpirationMinutes)
        };

        // Clean up expired entries
        CleanupExpiredEntries();
    }

    public bool IsPasswordVerified(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return false;
        }

        if (_verifiedClients.TryGetValue(clientId, out var info))
        {
            // Check if verification has expired
            if (info.ExpiresAt > DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                // Remove expired entry
                _verifiedClients.TryRemove(clientId, out _);
                return false;
            }
        }

        return false;
    }

    public void ClearVerification(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return;
        }

        _verifiedClients.TryRemove(clientId, out _);
    }

    private void CleanupExpiredEntries()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _verifiedClients
            .Where(kvp => kvp.Value.ExpiresAt <= now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _verifiedClients.TryRemove(key, out _);
        }
    }

    private class VerificationInfo
    {
        public DateTime VerifiedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
