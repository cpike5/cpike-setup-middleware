using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// In-memory implementation of <see cref="ISetupStateManager"/> that stores wizard state
/// in a thread-safe dictionary. State is scoped to the current request/wizard session.
/// </summary>
public class SetupStateManager : ISetupStateManager
{
    private readonly ConcurrentDictionary<string, object> _state = new();
    private readonly ILogger<SetupStateManager> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetupStateManager"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SetupStateManager(ILogger<SetupStateManager> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        if (value == null)
        {
            _state.TryRemove(key, out _);
            _logger.LogDebug("Removed state value for key: {Key}", key);
        }
        else
        {
            _state[key] = value;
            _logger.LogDebug("Set state value for key: {Key} (Type: {Type})", key, typeof(T).Name);
        }
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        var found = _state.TryGetValue(key, out var value) && value is T typedValue;

        if (found)
        {
            _logger.LogTrace("Retrieved state value for key: {Key}", key);
        }
        else
        {
            _logger.LogTrace("State value not found or type mismatch for key: {Key} (Expected type: {Type})", key, typeof(T).Name);
        }

        return found ? (T?)value : default;
    }

    /// <inheritdoc />
    public bool TryGet<T>(string key, out T? value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        if (_state.TryGetValue(key, out var obj) && obj is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool Contains(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        return _state.ContainsKey(key);
    }

    /// <inheritdoc />
    public bool Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        var removed = _state.TryRemove(key, out _);

        if (removed)
        {
            _logger.LogDebug("Removed state value for key: {Key}", key);
        }

        return removed;
    }

    /// <inheritdoc />
    public void Clear()
    {
        var keyCount = _state.Count;
        _state.Clear();
        _logger.LogDebug("Cleared all state values. Removed {Count} entries", keyCount);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetKeys()
    {
        return _state.Keys.ToList();
    }
}
