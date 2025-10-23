using System.Collections.Concurrent;

namespace cpike.Setup.Middleware.Services;

/// <summary>
/// In-memory implementation of <see cref="ISetupStateManager"/> that stores wizard state
/// in a thread-safe dictionary. State is scoped to the current request/wizard session.
/// </summary>
public class SetupStateManager : ISetupStateManager
{
    private readonly ConcurrentDictionary<string, object> _state = new();

    /// <inheritdoc />
    public void Set<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        if (value == null)
        {
            _state.TryRemove(key, out _);
        }
        else
        {
            _state[key] = value;
        }
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        return _state.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default;
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

        return _state.TryRemove(key, out _);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _state.Clear();
    }

    /// <inheritdoc />
    public IEnumerable<string> GetKeys()
    {
        return _state.Keys.ToList();
    }
}
