namespace SearchRank.Domain.Interfaces;

public interface ISearchEngineService
{
    /// <summary>
    /// Executes a search using the specified keyword and retrieves a collection of result URLs.
    /// </summary>
    /// <param name="keyword">The search keyword to query on the search engine.</param>
    /// <param name="maxResults">
    /// The maximum number of search results to retrieve from the engine.
    /// By default, this is set to 100, meaning the first 100 results will be considered.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection
    /// of result URLs (as strings) in the order they are presented by the search engine.
    /// </returns>
    Task<ICollection<string>?> SearchAsync(string keyword, int maxResults = 100);
}