using System.Web;
using HtmlAgilityPack;
using SearchRank.Domain.Interfaces;

namespace SearchRank.Infrastructure.Services;

public class GoogleSearchService(HttpClient httpClient) : SearchEngineService(httpClient), IGoogleSearchService
{
    /// <summary>
    /// Constructs the Google search URL by URL-encoding the keyword and using the 'num' parameter.
    /// </summary>
    /// <param name="keyword">The search term.</param>
    /// <param name="maxResults">The maximum number of results (up to 100 for Google).</param>
    /// <returns>The URL to query Google with.</returns>
    protected override string BuildSearchUrl(string keyword, int maxResults)
    {
        var encodedKeyword = HttpUtility.UrlEncode(keyword);
        return $"https://www.google.com/search?q={encodedKeyword}&num={maxResults}";
    }

    /// <summary>
    /// Uses HtmlAgilityPack to parse the HTML content returned by Google and extract search result URLs.
    /// </summary>
    /// <param name="html">The HTML content as a string.</param>
    /// <returns>A collection of URLs in the order they appear.</returns>
    protected override IEnumerable<string> ParseHtmlForResults(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//div[@class='yuRUbf']/a");
        var urls = nodes.Select(node => node.GetAttributeValue("href", string.Empty)).Where(url => !string.IsNullOrWhiteSpace(url));
        return urls;
    }
}