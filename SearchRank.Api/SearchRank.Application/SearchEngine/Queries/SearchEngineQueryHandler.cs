using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using SearchRank.Domain.Entities;
using SearchRank.Domain.Enums;
using SearchRank.Domain.Interfaces;
using SearchRank.Domain.Models;

namespace SearchRank.Application.SearchEngine.Queries;

public class SearchEngineQueryHandler(
    IGoogleSearchService googleSearchService,
    IBingSearchService bingSearchService,
    ICacheService cacheService,
    IQueryLogRepository queryLogRepository,
    ILogger<SearchEngineQueryHandler> logger)
    : IRequestHandler<SearchEngineQuery, OneOf<SearchEngineQuery.ResultModel, Error>>
{
    private const string GoogleCacheKey = "Google-Cache-Key:{0}";
    private const string BingCacheKey = "Bing-Cache-Key:{1}";
    
    public async Task<OneOf<SearchEngineQuery.ResultModel, Error>> Handle(SearchEngineQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing search query for keyword: {Keyword}, targetUrl: {TargetUrl}", request.Keyword, request.TargetUrl);
        
        var googleResults = await GetGoogleResultAsync(request.Keyword);
        var bingResults =  await GetBingResultAsync(request.Keyword);

        logger.LogInformation("Retrieved {GoogleCount} results from Google and {BingCount} results from Bing.", googleResults?.Count, bingResults?.Count);
        
        var googleRank = FindRank(googleResults, request.TargetUrl);
        var bingRank = FindRank(bingResults, request.TargetUrl);
        
        logger.LogInformation("Search rank determined. Google: {GoogleRank}, Bing: {BingRank}", googleRank, bingRank);
        
        var searchQuery = new SearchQuery
        {
            Id = Guid.NewGuid(),
            Keyword = request.Keyword,
            TargetUrl = request.TargetUrl,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
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
                new SearchEngineModel { SearchEngine = SearchEngineType.Google, Rank = googleRank },
                new SearchEngineModel { SearchEngine = SearchEngineType.Bing, Rank = bingRank }
            ]
        };
    }
    
    #region Private Method(s)

    private async Task<ICollection<string>?> GetGoogleResultAsync(string keyword)
    {
        if (cacheService.TryGetValue(GoogleCacheKey, out ICollection<string>? result)) return result;
        result = await googleSearchService.SearchAsync(keyword);
        cacheService.Set(GoogleCacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }
    
    private async Task<ICollection<string>?> GetBingResultAsync(string keyword)
    {
        if (cacheService.TryGetValue(BingCacheKey, out ICollection<string>? result)) return result;
        result = await bingSearchService.SearchAsync(keyword);
        cacheService.Set(BingCacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }
    
    /// <summary>
    /// Finds the ranking position of the target URL in a sequence of search result URLs.
    /// </summary>
    /// <param name="searchResults">A sequence of URLs from search results.</param>
    /// <param name="targetUrl">The target URL to locate.</param>
    /// <returns>
    /// The 1-indexed rank if found; otherwise, null.
    /// </returns>
    private static int? FindRank(ICollection<string>? searchResults, string targetUrl)
    {
        if (searchResults == null) return null;
        
        var index = 1;
        foreach (var url in searchResults)
        {
            if (NormalizeUrl(url) == NormalizeUrl(targetUrl))
            {
                return index;
            }

            index++;
        }

        return null;
    }

    /// <summary>
    /// Normalizes the URL for comparison, such as by trimming and converting to lowercase.
    /// </summary>
    /// <param name="url">The URL to normalize.</param>
    /// <returns>A normalized URL string.</returns>
    private static string NormalizeUrl(string url) => url.Trim().ToLowerInvariant();
    
    #endregion
}