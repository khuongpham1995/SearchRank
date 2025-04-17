using SearchRank.Application.SearchEngine.Queries;
using SearchRank.Domain.Enums;
using SearchRank.Domain.Models;

namespace SearchRank.Presentation.Requests;

public record SearchRequest(string Keyword, string TargetUrl);

public static class SearchRequestExtensions
{
    public static SearchEngineQuery ToQuery(this SearchRequest input, SearchEngineType type)
    {
        return new SearchEngineQuery(input.Keyword, input.TargetUrl, type);
    }
}