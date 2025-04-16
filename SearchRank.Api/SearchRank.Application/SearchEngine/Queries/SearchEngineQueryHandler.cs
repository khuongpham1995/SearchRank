using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Entities;
using SearchRank.Domain.Enums;
using SearchRank.Domain.Interfaces;
using SearchRank.Domain.Models;

namespace SearchRank.Application.SearchEngine.Queries;

public class SearchEngineQueryHandler(
    ISearchEngineService searchEngineService,
    ICacheService cacheService,
    IQueryLogRepository queryLogRepository,
    ILogger<SearchEngineQueryHandler> logger)
    : IRequestHandler<SearchEngineQuery, OneOf<SearchEngineQuery.ResultModel, Error>>
{
    private const string CacheKey = "{0}-Cache-Key-{1}-{2}-{3}";
    
    public async Task<OneOf<SearchEngineQuery.ResultModel, Error>> Handle(SearchEngineQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing search query for keyword: {Keyword}, targetUrl: {TargetUrl}", request.Keyword, request.TargetUrl);

        var googleTask = GetGoogleResultAsync(request.UserId, request.Keyword, request.TargetUrl);
        var bingTask = GetBingResultAsync(request.UserId, request.Keyword, request.TargetUrl);
        await Task.WhenAll(new List<Task> { googleTask, bingTask });
        var googleResults = await googleTask;
        var bingResults = await bingTask;

        logger.LogInformation("Retrieved {GoogleCount} results from Google and {BingCount} results from Bing.", googleResults?.Count, bingResults?.Count);
        
        var googleRank = FindRank(googleResults);
        var bingRank = FindRank(bingResults);
        
        logger.LogInformation("Search rank determined. Google: {GoogleRank}, Bing: {BingRank}", googleRank, bingRank);
        
        var searchQuery = new SearchQuery(request.Keyword, request.TargetUrl, request.UserId)
        {
            GoogleRank = googleRank,
            BingRank = bingRank
        };
        
        await queryLogRepository.AddAsync(searchQuery);
        logger.LogInformation("Logged search query for user {UserId} at {Timestamp}", request.UserId, searchQuery.CreatedAt);
        
        return new SearchEngineQuery.ResultModel
        {
            Keyword = request.Keyword,
            TargetUrl = request.TargetUrl,
            Timestamp = searchQuery.CreatedAt,
            Results =
            [
                new Domain.Models.SearchEngine { Type = SearchEngineType.Google, Rank = googleRank },
                new Domain.Models.SearchEngine { Type = SearchEngineType.Bing, Rank = bingRank }
            ]
        };
    }
    
    #region Private Method(s)

    private async Task<ICollection<int>?> GetGoogleResultAsync(Guid userId, string keyword, string targetUrl)
    {
        var cacheKey = string.Format(CacheKey, SearchEngineType.Google, userId, keyword, targetUrl);
        if (cacheService.TryGetValue(cacheKey, out ICollection<int>? result)) return result;
        result = await searchEngineService.SearchGoogleAsync(keyword, targetUrl);
        cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CommonConstant.CacheExpireMinute));
        return result;
    }
    
    private async Task<ICollection<int>?> GetBingResultAsync(Guid userId, string keyword, string targetUrl)
    {
        var cacheKey = string.Format(CacheKey, SearchEngineType.Bing, userId, keyword, targetUrl);
        if (cacheService.TryGetValue(cacheKey, out ICollection<int>? result)) return result;
        result = await searchEngineService.SearchBingAsync(keyword, targetUrl, resultsPerPage: 25);
        cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CommonConstant.CacheExpireMinute));
        return result;
    }

    /// <summary>
    /// Determines the best ranking (lowest ranking value) from a collection of ranking positions.
    /// </summary>
    /// <param name="searchResults">A collection of ranking positions (1-indexed) returned from a search engine.</param>
    /// <returns>
    /// The minimum ranking value if available; otherwise, null.
    /// </returns>
    private static int? FindRank(ICollection<int>? searchResults) => searchResults?.Any() == true ? searchResults.Min() : null;

    #endregion
}