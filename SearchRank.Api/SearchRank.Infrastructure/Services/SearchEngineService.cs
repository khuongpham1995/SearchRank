using SearchRank.Domain.Interfaces;

namespace SearchRank.Infrastructure.Services;

public abstract class SearchEngineService(HttpClient httpClient) : ISearchEngineService
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Constructs the search URL for the specific search engine, based on the keyword and maximum results.
    /// </summary>
    /// <param name="keyword">The search term to query for.</param>
    /// <param name="maxResults">The maximum number of results to return.</param>
    /// <returns>The fully built URL to fetch the search results.</returns>
    protected abstract string BuildSearchUrl(string keyword, int maxResults);

    /// <summary>
    /// Parses the HTML content returned from the search engine and extracts the list of result URLs.
    /// </summary>
    /// <param name="html">The HTML content as a string.</param>
    /// <returns>A collection of URLs extracted in the order they appear in the search results.</returns>
    protected abstract IEnumerable<string> ParseHtmlForResults(string html);
    
    public async Task<ICollection<string>?> SearchAsync(string keyword, int maxResults = 100)
    {
        var url = BuildSearchUrl(keyword, maxResults);
        if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");
        }
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var htmlContent = await response.Content.ReadAsStringAsync();
        var results = ParseHtmlForResults(htmlContent);
        return results.Take(maxResults).ToList();
    }
}