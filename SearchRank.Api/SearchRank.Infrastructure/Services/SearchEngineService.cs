using Microsoft.Extensions.Logging;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Extensions;

namespace SearchRank.Infrastructure.Services;

public class SearchEngineService(ILogger<SearchEngineService> logger) : ISearchEngineService
{
    public async Task<ICollection<int>> SearchBingAsync(string keyword, string url, int resultsPerPage)
    {
        var result = new List<int>();
        var totalPages = CommonConstant.MaxTotalResults / resultsPerPage;

        for (var page = 0; page < totalPages; page++)
        {
            var offset = page * resultsPerPage + 1;
            var queryUrl = $"{CommonConstant.BingUrl}?q={Uri.EscapeDataString(keyword)}&first={offset}";
            var htmlContent = await SearchAsync(queryUrl);
            if (string.IsNullOrEmpty(htmlContent)) continue;

            var pagePositions = HtmlExtension.FindUrlPositionsForBing(htmlContent, url);
            result.AddRange(pagePositions.Select(p => p + offset - 1));
        }

        return result;
    }

    public async Task<ICollection<int>> SearchGoogleAsync(string keyword, string url)
    {
        var queryUrl =
            $"{CommonConstant.GoogleUrl}?q={Uri.EscapeDataString(keyword)}&num={CommonConstant.MaxTotalResults}";
        var htmlContent = await SearchAsync(queryUrl);
        return string.IsNullOrEmpty(htmlContent) ? [] : HtmlExtension.FindUrlPositionsForGoogle(htmlContent, url);
    }

    #region Private Method(s)

    private async Task<string> SearchAsync(string url)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(CommonConstant.UserAgentHeader);
        httpClient.DefaultRequestHeaders.Accept.ParseAdd(CommonConstant.AcceptHeader);
        httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CommonConstant.AcceptLanguageHeader);
        httpClient.DefaultRequestHeaders.Connection.ParseAdd(CommonConstant.ConnectionHeader);
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();
        logger.LogError("Failed to fetch data from {Url}. Status code: {StatusCode}", url, response.StatusCode);
        return string.Empty;
    }

    #endregion
}