using System.ComponentModel.DataAnnotations;

namespace SearchRank.Domain.Entities;

public class SearchQuery
{
    public SearchQuery()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    public SearchQuery(string keyword, string targetUrl, Guid userId)
    {
        Id = Guid.NewGuid();
        Keyword = keyword;
        TargetUrl = targetUrl;
        UserId = userId;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    public Guid Id { get; set; }
    public string? Keyword { get; set; }
    public string? TargetUrl { get; set; }
    public Guid UserId { get; set; }
    public int? GoogleRank { get; set; }
    public int? BingRank { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = [];

    #region Navigations
    
    public User User { get; set; }

    #endregion
}