namespace SearchRank.Domain.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// Attempts to retrieve a cached value by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="value">
    /// When this method returns, contains the cached value associated with the specified key, 
    /// if the key is found; otherwise, the default value for the type of the value parameter.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the cache contains an entry for the specified key; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool TryGetValue<T>(string key, out T? value);

    /// <summary>
    /// Stores the specified value in the cache with an absolute expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to be cached.</typeparam>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="absoluteExpirationRelativeToNow">
    /// The time interval after which the cache entry should expire.
    /// </param>
    void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);

    /// <summary>
    /// Removes the cache entry associated with the specified key, if it exists.
    /// </summary>
    /// <param name="key">The unique key identifying the cache entry to remove.</param>
    void Remove(string key);
}