using Microsoft.Extensions.Logging;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Extensions;

namespace SearchRank.Infrastructure.Services;

public class SearchEngineService(HttpClient httpClient) : ISearchEngineService
{
    public async Task<ICollection<int>> SearchBingAsync(string keyword, string url, int resultsPerPage)
    {
        var result = new List<int>();
        int totalPages = CommonConstant.MaxTotalResults / resultsPerPage;

        for (int page = 0; page < totalPages; page++)
        {
            int offset = page * resultsPerPage + 1;
            var queryUrl = $"{CommonConstant.BingUrl}?q={Uri.EscapeDataString(keyword)}&first={offset}";
            var htmlContent = await SearchAsync(queryUrl);
            if (string.IsNullOrEmpty(htmlContent))
            {
                continue;
            }

            var pagePositions = HtmlExtension.FindUrlPositionsForBing(htmlContent, url);
            result.AddRange(pagePositions.Select(p => p + offset - 1));
        }

        return result;
    }

    public async Task<ICollection<int>> SearchGoogleAsync(string keyword, string url)
    {
        var queryUrl = $"{CommonConstant.GoogleUrl}?q={Uri.EscapeDataString(keyword)}&num={CommonConstant.MaxTotalResults}";
        var htmlContent = await SearchAsync(queryUrl);
        if (string.IsNullOrEmpty(htmlContent))
        {
            return [];
        }
        return HtmlExtension.FindUrlPositionsForGoogle(htmlContent, url);
    }

    #region Private Method(s)

    private async Task<string> SearchAsync(string url)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(CommonConstant.DefaultUserAgent);
        var response = await httpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    #endregion

}