using SearchRank.Domain.Enums;

namespace SearchRank.Domain.Models;

public class SearchEngine
{
    public SearchEngineType Type { get; set; }
    public string? Rank { get; set; }
}