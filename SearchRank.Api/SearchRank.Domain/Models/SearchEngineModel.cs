using SearchRank.Domain.Enums;

namespace SearchRank.Domain.Models;
public class SearchEngineModel
{
    public SearchEngineType SearchEngine { get; set; }
    public int? Rank { get; set; }
}