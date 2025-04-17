namespace SearchRank.Presentation;

public class AppConfig
{
    public ApiAction ApiAction { get; set; } = new();
    public JwtSettings JwtSettings { get; set; } = new();
    public RateLimiterSettings RateLimiter { get; set; } = new();
    public required CorsSettings CorsSettings { get; set; }
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

public class RateLimiterSettings
{
    public int PermitLimit { get; set; }
    public int FixedWindowSecond { get; set; }
    public int QueueLimit { get; set; }
}

public class ApiAction
{
    public string Grouping { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Ranking { get; set; } = string.Empty;
    public string GoogleRank => string.IsNullOrEmpty(Ranking) ? string.Empty : $"/google{Ranking}";
    public string BingRank => string.IsNullOrEmpty(Ranking) ? string.Empty : $"/bing{Ranking}";
    public string Log { get; set; } = string.Empty;
}

public class CorsSettings
{
    public required string[] AllowedOrigins { get; set; }
}