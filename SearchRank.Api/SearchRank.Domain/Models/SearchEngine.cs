using SearchRank.Domain.Enums;

namespace SearchRank.Domain.Models;

public class SearchEngine
{
    public SearchEngineType Type { get; set; }
    public int? Rank { get; set; }
}