namespace cpike.Setup.Middleware.Services;

/// <summary>
/// Manages state data that is shared between setup steps during wizard execution.
/// State is scoped to the current wizard session.
/// </summary>
public interface ISetupStateManager
{
    /// <summary>
    /// Stores a value in the wizard state with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The unique key for this value.</param>
    /// <param name="value">The value to store.</param>
    void Set<T>(string key, T value);

    /// <summary>
    /// Retrieves a value from the wizard state by key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The unique key for the value.</param>
    /// <returns>The value if found; otherwise, the default value for type T.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Attempts to retrieve a value from the wizard state by key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The unique key for the value.</param>
    /// <param name="value">The value if found; otherwise, the default value for type T.</param>
    /// <returns>True if the value was found; otherwise, false.</returns>
    bool TryGet<T>(string key, out T? value);

    /// <summary>
    /// Checks if a value exists in the wizard state with the specified key.
    /// </summary>
    /// <param name="key">The unique key to check.</param>
    /// <returns>True if the key exists; otherwise, false.</returns>
    bool Contains(string key);

    /// <summary>
    /// Removes a value from the wizard state by key.
    /// </summary>
    /// <param name="key">The unique key of the value to remove.</param>
    /// <returns>True if the value was removed; otherwise, false.</returns>
    bool Remove(string key);

    /// <summary>
    /// Clears all values from the wizard state.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets all keys currently stored in the wizard state.
    /// </summary>
    /// <returns>A collection of all state keys.</returns>
    IEnumerable<string> GetKeys();
}
