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

        var googleTask = GetGoogleResultAsync(request.Keyword, request.TargetUrl);
        var bingTask = GetBingResultAsync(request.Keyword, request.TargetUrl);
        await Task.WhenAll(new List<Task> { googleTask, bingTask });
        var googleResults = await googleTask;
        var bingResults = await bingTask;

        logger.LogInformation("Retrieved {GoogleCount} results from Google and {BingCount} results from Bing.",
            googleResults?.Count, bingResults?.Count);

        var googleRank = FindRank(googleResults);
        var bingRank = FindRank(bingResults);

        logger.LogInformation("Search rank determined. Google: {GoogleRank}, Bing: {BingRank}", googleRank, bingRank);

        return new SearchEngineQuery.ResultModel
        {
            Keyword = request.Keyword,
            TargetUrl = request.TargetUrl,
            Timestamp = DateTimeOffset.Now,
            Results =
            [
                new Domain.Models.SearchEngine { Type = SearchEngineType.Google, Rank = googleRank },
                new Domain.Models.SearchEngine { Type = SearchEngineType.Bing, Rank = bingRank }
            ]
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