namespace SearchRank.Domain.Interfaces;

public interface ISearchEngineService
{
    /// <summary>
    ///     Executes a search query on Bing with the specified keyword and target URL,
    ///     retrieving a collection of ranking positions for the target URL within the search results.
    /// </summary>
    /// <param name="keyword">The search keyword to query Bing.</param>
    /// <param name="url">The target URL whose ranking position is to be determined.</param>
    /// <param name="resultsPerPage">The number of search results to retrieve per page.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a collection
    ///     of integers representing the ranking positions (1-indexed) at which the target URL appears
    ///     in the Bing search results.
    /// </returns>
    Task<IReadOnlyCollection<int>> SearchBingAsync(string keyword, string url, int resultsPerPage);

    /// <summary>
    ///     Executes a search query on Google with the specified keyword and target URL,
    ///     retrieving a collection of ranking positions for the target URL within the search results.
    /// </summary>
    /// <param name="keyword">The search keyword to query Google.</param>
    /// <param name="url">The target URL whose ranking position is to be determined.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a collection
    ///     of integers representing the ranking positions (1-indexed) at which the target URL appears
    ///     in the Google search results.
    /// </returns>
    Task<IReadOnlyCollection<int>> SearchGoogleAsync(string keyword, string url);
}