using SearchRank.Application.SearchEngine.Queries;

namespace SearchRank.Presentation.Requests;

public record SearchRequest(string Keyword, string TargetUrl, Guid UserId);

public static class SearchRequestExtensions
{
    public static SearchEngineQuery ToQuery(this SearchRequest input)
    {
        return new SearchEngineQuery(input.Keyword, input.TargetUrl, input.UserId);
    }
}