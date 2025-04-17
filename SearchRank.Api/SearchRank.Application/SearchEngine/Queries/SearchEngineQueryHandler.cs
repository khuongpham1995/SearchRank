using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Enums;
using SearchRank.Domain.Interfaces;
using SearchRank.Domain.Models;

namespace SearchRank.Application.SearchEngine.Queries;

public class SearchEngineQueryHandler(
    ISearchEngineService searchEngineService,
    ICacheService cacheService,
    ILogger<SearchEngineQueryHandler> logger)
    : IRequestHandler<SearchEngineQuery, OneOf<SearchEngineQuery.ResultModel, Error>>
{
    public async Task<OneOf<SearchEngineQuery.ResultModel, Error>> Handle(SearchEngineQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TargetUrl))
        {
            logger.LogWarning("Search attempt failed: Invalid TargetUrl");
            return new Error("TargetUrl is required");
        }

        if (string.IsNullOrWhiteSpace(request.Keyword))
        {
            logger.LogWarning("Search attempt failed: Invalid Keyword");
            return new Error("Keyword is required");
        }

        logger.LogInformation("Processing search query for keyword: {Keyword}, targetUrl: {TargetUrl}", request.Keyword,
            request.TargetUrl);

        var result = request.SearchEngineType switch
        {
            SearchEngineType.Google => await GetGoogleResultAsync(request.Keyword, request.TargetUrl),
            SearchEngineType.Bing => await GetBingResultAsync(request.Keyword, request.TargetUrl),
            _ => throw new ArgumentOutOfRangeException(nameof(request.SearchEngineType), "Invalid search engine type")
        };
        logger.LogInformation("Retrieved {Count} results from {SearchEngine}.", result?.Count, request.SearchEngineType.ToString());
        return new SearchEngineQuery.ResultModel
        {
            Keyword = request.Keyword,
            TargetUrl = request.TargetUrl,
            Timestamp = DateTimeOffset.Now,
            Result = new Domain.Models.SearchEngine { Type = request.SearchEngineType, Rank = FindRank(result) }
        };
    }

    #region Private Method(s)

    private async Task<ICollection<int>?> GetGoogleResultAsync(string keyword, string targetUrl)
    {
        var cacheKey = string.Format(CommonConstant.CacheKeyFormat, SearchEngineType.Google, keyword, targetUrl);
        if (cacheService.TryGetValue(cacheKey, out ICollection<int>? result)) return result;
        result = await searchEngineService.SearchGoogleAsync(keyword, targetUrl);
        cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CommonConstant.CacheExpireMinute));
        return result;
    }

    private async Task<ICollection<int>?> GetBingResultAsync(string keyword, string targetUrl)
    {
        var cacheKey = string.Format(CommonConstant.CacheKeyFormat, SearchEngineType.Bing, keyword, targetUrl);
        if (cacheService.TryGetValue(cacheKey, out ICollection<int>? result)) return result;
        result = await searchEngineService.SearchBingAsync(keyword, targetUrl, 25);
        cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CommonConstant.CacheExpireMinute));
        return result;
    }

    /// <summary>
    ///     Determines the best ranking (lowest ranking value) from a collection of ranking positions.
    /// </summary>
    /// <param name="searchResults">A collection of ranking positions (1-indexed) returned from a search engine.</param>
    /// <returns>
    ///     The minimum ranking value if available; otherwise, null.
    /// </returns>
    private static string FindRank(ICollection<int>? searchResults)
    {
        return searchResults?.Any() == true ? string.Join(", ", searchResults) : "0";
    }

    #endregion
}