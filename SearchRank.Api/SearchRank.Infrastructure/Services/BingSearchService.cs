using System.Web;
using HtmlAgilityPack;
using SearchRank.Domain.Interfaces;

namespace SearchRank.Infrastructure.Services;

public class BingSearchService(HttpClient httpClient) : SearchEngineService(httpClient), IBingSearchService
{
    /// <summary>
    /// Constructs the Bing search URL by URL-encoding the keyword and using count and offset parameters.
    /// Note: Bing typically allows a maximum of 50 results per page.
    /// </summary>
    /// <param name="keyword">The search term.</param>
    /// <param name="maxResults">The maximum number of results to retrieve.</param>
    /// <returns>The URL to query Bing with.</returns>
    protected override string BuildSearchUrl(string keyword, int maxResults)
    {
        var count = maxResults > 50 ? 50 : maxResults;
        var encodedKeyword = HttpUtility.UrlEncode(keyword);
        return $"https://www.bing.com/search?q={encodedKeyword}&count={count}&first=1";
    }

    /// <summary>
    /// Uses HtmlAgilityPack to parse the HTML content returned by Bing and extract search result URLs.
    /// </summary>
    /// <param name="html">The HTML content as a string.</param>
    /// <returns>A collection of URLs in the order they appear in Bing's search results.</returns>
    protected override IEnumerable<string> ParseHtmlForResults(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//li[@class='b_algo']//h2/a");
        var urls = nodes.Select(node => node.GetAttributeValue("href", string.Empty)).Where(url => !string.IsNullOrWhiteSpace(url));
        return urls;
    }
}