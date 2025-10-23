# Setup Password Protection

## Overview

The setup wizard can be protected with a generated password to prevent unauthorized access during public deployments. This feature is enabled by default for security.

## How It Works

### 1. Password Generation

When the application starts and setup is incomplete:

1. The middleware generates a secure random password
2. The password is logged to the console
3. Optionally, the password is written to a file
4. The password is valid for the current application session

### 2. Password Display

**Console Output:**

```
========================================
SETUP WIZARD ACCESS PASSWORD
========================================
Your setup wizard is protected.
Enter this password to begin setup:

    XpK7-mN94-Qr2L-vB8j

This password is valid for this session only.
For security, it has also been written to:
    /app_data/setup-password.txt
========================================
```

**File Output (Optional):**

The password can be written to a configurable file location for environments where console access is limited.

### 3. Password Entry

When accessing `/setup`, the user sees a password prompt:

```
┌─────────────────────────────────┐
│     Setup Wizard Protection     │
├─────────────────────────────────┤
│                                 │
│  Please enter the setup         │
│  password to continue.          │
│                                 │
│  The password was displayed     │
│  when the application started.  │
│                                 │
│  [____________________]         │
│                                 │
│         [  Continue  ]          │
│                                 │
└─────────────────────────────────┘
```

### 4. Password Validation

- Password must match exactly (case-sensitive)
- Rate limiting prevents brute force (5 attempts per 5 minutes)
- Invalid attempts are logged
- After successful entry, password is valid for the session

## Configuration

### Enable/Disable Password Protection

**appsettings.json:**

```json
{
  "Setup": {
    "RequirePassword": true,  // Default: true
    "PasswordOutputFile": "/app_data/setup-password.txt",
    "PasswordOutputToConsole": true
  }
}
```

**Program.cs:**

```csharp
builder.Services.AddSetupWizard(setup =>
{
    setup.WithOptions(options =>
    {
        options.RequirePassword = true;
        options.PasswordOutputFile = "/app_data/setup-password.txt";
        options.PasswordOutputToConsole = true;
    });
});
```

### Disable Password Protection

For development or private environments where password protection isn't needed:

```json
{
  "Setup": {
    "RequirePassword": false
  }
}
```

### Custom Password

For testing or specific scenarios, you can provide a custom password:

```json
{
  "Setup": {
    "RequirePassword": true,
    "CustomPassword": "your-custom-password"
  }
}
```

**Warning:** Custom passwords should only be used for testing. Always use generated passwords in production.

## Security Considerations

### Password Strength

Generated passwords are:

- 20 characters long
- Use alphanumeric characters (A-Z, a-z, 0-9)
- Include hyphens for readability
- Generated using cryptographically secure random number generator
- Format: `XXXX-XXXX-XXXX-XXXX` (e.g., `XpK7-mN94-Qr2L-vB8j`)

### Rate Limiting

Password entry is rate-limited to prevent brute force attacks:

- Maximum 5 attempts per 5-minute window
- Attempts tracked by IP address
- Lockout duration: 5 minutes
- Failed attempts logged for monitoring

### Session Management

- Password validated once per session
- Session uses secure HTTP-only cookies
- Session expires on browser close
- Session timeout: 2 hours (configurable)

### Password File Security

If using file output:

- File created with restricted permissions (owner read-only)
- File stored outside web root
- File deleted after setup completion (optional)
- Location configurable to use secure storage

## Use Cases

### Scenario 1: Development Environment

**Configuration:**

```json
{
  "Setup": {
    "RequirePassword": false
  }
}
```

**Reason:** Fast iteration, no security concerns on localhost.

### Scenario 2: Production Deployment

**Configuration:**

```json
{
  "Setup": {
    "RequirePassword": true,
    "PasswordOutputToConsole": true,
    "PasswordOutputFile": "/secure/setup-password.txt"
  }
}
```

**Reason:** Maximum security for publicly accessible deployments.

### Scenario 3: Docker/Kubernetes

**Configuration:**

```json
{
  "Setup": {
    "RequirePassword": true,
    "PasswordOutputToConsole": true,
    "PasswordOutputFile": "/mnt/secrets/setup-password.txt"
  }
}
```

**Access Password:**

```bash
# View logs to get password
docker logs <container-id>

# Or read from file
docker exec <container-id> cat /mnt/secrets/setup-password.txt

# Or with kubectl
kubectl logs <pod-name>
kubectl exec <pod-name> -- cat /mnt/secrets/setup-password.txt
```

### Scenario 4: CI/CD Automated Testing

**Configuration:**

```json
{
  "Setup": {
    "RequirePassword": true,
    "CustomPassword": "test-password-123"
  }
}
```

**Reason:** Predictable password for automated tests.

## Implementation Details

### Password Generation Algorithm

```csharp
public static string GenerateSetupPassword()
{
    const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
    var random = RandomNumberGenerator.Create();
    var bytes = new byte[20];
    random.GetBytes(bytes);

    var result = new StringBuilder(24);
    for (int i = 0; i < 20; i++)
    {
        result.Append(chars[bytes[i] % chars.Length]);

        // Add hyphen every 4 characters
        if ((i + 1) % 4 == 0 && i < 19)
        {
            result.Append('-');
        }
    }

    return result.ToString();
}
```

**Notes:**

- Excludes confusing characters (0, O, 1, I, l)
- Uses `RandomNumberGenerator` for cryptographic randomness
- Format matches industry standards (e.g., license keys)

### Rate Limiting Implementation

```csharp
public class SetupPasswordRateLimiter
{
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    public bool IsAllowed(string ipAddress)
    {
        var key = $"setup_password_attempts_{ipAddress}";

        if (_cache.TryGetValue(key, out int attempts))
        {
            return attempts < MaxAttempts;
        }

        return true;
    }

    public void RecordAttempt(string ipAddress, bool success)
    {
        if (success)
        {
            // Clear attempts on success
            _cache.Remove($"setup_password_attempts_{ipAddress}");
            return;
        }

        var key = $"setup_password_attempts_{ipAddress}";
        var attempts = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = LockoutDuration;
            return 0;
        });

        _cache.Set(key, attempts + 1, LockoutDuration);
    }
}
```

### Password Storage

Passwords are **never** stored in:

- Database
- Configuration files (except custom passwords for testing)
- Session state
- Cookies (except session ID)
- Application logs (only shown, not logged)

Passwords exist only:

- In memory during application session
- In console output (ephemeral)
- In password file (if enabled, temporary)

## UI/UX

### Password Entry Screen

```
┌─────────────────────────────────────────────┐
│                                             │
│           🔒 Setup Wizard Protection        │
│                                             │
│  This setup wizard is protected to prevent  │
│  unauthorized access.                       │
│                                             │
│  Please enter the setup password that was   │
│  displayed when the application started.    │
│                                             │
│  ┌─────────────────────────────────────┐   │
│  │ Password                            │   │
│  │ [____________________________]      │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  ⓘ Check the console logs or the file:     │
│     /app_data/setup-password.txt            │
│                                             │
│           [  Continue to Setup  ]           │
│                                             │
└─────────────────────────────────────────────┘
```

### Error Messages

**Invalid Password:**

```
┌─────────────────────────────────────────────┐
│  ❌ Invalid password                        │
│                                             │
│  The password you entered is incorrect.     │
│  Please check the console or password file. │
│                                             │
│  Attempts remaining: 4/5                    │
└─────────────────────────────────────────────┘
```

**Rate Limited:**

```
┌─────────────────────────────────────────────┐
│  ⚠️ Too many attempts                       │
│                                             │
│  You've exceeded the maximum number of      │
│  password attempts. Please try again in:    │
│                                             │
│          4 minutes, 32 seconds              │
│                                             │
└─────────────────────────────────────────────┘
```

## Troubleshooting

### Problem: Can't Find Password

**Solutions:**

1. Check console/terminal where application started
2. Check password file location (default: `/app_data/setup-password.txt`)
3. Restart application to generate new password
4. Temporarily disable password requirement in config

### Problem: Password Not Working

**Check:**

1. Password is case-sensitive (exact match required)
2. No extra spaces before/after password
3. Including hyphens in password
4. Not rate-limited (too many failed attempts)

### Problem: Password File Not Created

**Check:**

1. File path is valid and writable
2. Application has permissions to create file
3. Directory exists
4. `PasswordOutputFile` is configured

### Problem: Rate Limited Unfairly

**Solutions:**

1. Wait for lockout period to expire (5 minutes)
2. Restart application to reset rate limiter
3. Configure custom password for testing
4. Check IP address detection if behind proxy

## Best Practices

### Production Deployments

1. **Keep password protection enabled**
   ```json
   { "Setup": { "RequirePassword": true } }
   ```

2. **Capture startup logs**
   ```bash
   # Docker
   docker logs <container> > startup.log

   # Systemd
   journalctl -u your-app.service > startup.log
   ```

3. **Secure password file**
   ```bash
   # Set restrictive permissions
   chmod 600 /app_data/setup-password.txt

   # Delete after setup
   rm /app_data/setup-password.txt
   ```

4. **Use secrets management for custom passwords**
   ```bash
   # Environment variable
   Setup__CustomPassword="${SETUP_PASSWORD}"

   # Kubernetes secret
   kubectl create secret generic setup-password \
     --from-literal=password=your-password
   ```

### Development Workflows

1. **Disable for local development**
   ```json
   // appsettings.Development.json
   { "Setup": { "RequirePassword": false } }
   ```

2. **Use consistent custom password for team**
   ```json
   // appsettings.Development.json (shared)
   { "Setup": { "CustomPassword": "dev-password" } }
   ```

3. **Document password requirement in README**
   ```markdown
   ## First-Time Setup

   When running for the first time, you'll need the setup password.
   Check the console output or `/app_data/setup-password.txt`.
   ```

## Security Audit Checklist

- [ ] Password protection enabled in production
- [ ] Generated passwords are cryptographically random
- [ ] Rate limiting is active and tested
- [ ] Password file has restricted permissions
- [ ] Password file deleted after setup (if applicable)
- [ ] Failed attempts are logged
- [ ] No passwords in version control
- [ ] No passwords in application logs
- [ ] Session cookies are HTTP-only and secure
- [ ] Setup completion disables password protection

## API Reference

### SetupOptions

```csharp
public class SetupOptions
{
    /// <summary>
    /// Require password to access setup wizard.
    /// Default: true
    /// </summary>
    public bool RequirePassword { get; set; } = true;

    /// <summary>
    /// Output password to console on startup.
    /// Default: true
    /// </summary>
    public bool PasswordOutputToConsole { get; set; } = true;

    /// <summary>
    /// File path to write password.
    /// Default: "/app_data/setup-password.txt"
    /// </summary>
    public string PasswordOutputFile { get; set; } = "/app_data/setup-password.txt";

    /// <summary>
    /// Custom password (for testing only).
    /// Default: null (generated)
    /// </summary>
    public string CustomPassword { get; set; }

    /// <summary>
    /// Delete password file after setup completion.
    /// Default: true
    /// </summary>
    public bool DeletePasswordFileAfterSetup { get; set; } = true;

    /// <summary>
    /// Maximum password attempts before lockout.
    /// Default: 5
    /// </summary>
    public int MaxPasswordAttempts { get; set; } = 5;

    /// <summary>
    /// Lockout duration after max attempts.
    /// Default: 5 minutes
    /// </summary>
    public TimeSpan PasswordLockoutDuration { get; set; } = TimeSpan.FromMinutes(5);
}
```

## Future Enhancements

### Planned Features

1. **Time-based password expiration**
   - Password expires after X hours
   - Prevents stale passwords in long-running containers

2. **Multiple password formats**
   - Numeric PIN
   - Phrase-based passwords
   - QR code for mobile scanning

3. **Email delivery**
   - Send password to configured admin email
   - Requires SMTP configuration

4. **Webhook notification**
   - POST password to webhook URL
   - Integration with Slack, Teams, etc.

5. **2FA option**
   - TOTP-based two-factor authentication
   - For extra-sensitive deployments
