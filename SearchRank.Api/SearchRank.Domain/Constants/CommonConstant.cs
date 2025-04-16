namespace SearchRank.Domain.Constants;

public static class CommonConstant
{
    public const int CacheExpireMinute = 15;
    public const string CacheKeyFormat = "{0}-CacheKey-{1}-{2}";
    public const int MaxTotalResults = 100;
    public const string GoogleUrl = "https://www.google.com/search";
    public const string BingUrl = "https://www.bing.com/search";
    
    public const string UserAgentHeader =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Edg/135.0.0.0 Safari/537.36";
    public const string AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
    public const string AcceptLanguageHeader = "en-US,en;q=0.5";
    public const string ConnectionHeader = "keep-alive";
    
    public const string FixedRateLimitingPolicy = "rest-api-rate-limiting";
    public const string AllowAngularApp = "search-rank-client";
}