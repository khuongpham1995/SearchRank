namespace SearchRank.Presentation;

public class AppConfig
{
    public ApiAction ApiAction { get; set; } = new();
    public JwtSettings JwtSettings { get; set; } = new();
    public RateLimiterSettings RateLimiter { get; set; } = new();
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
}