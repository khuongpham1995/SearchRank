using Microsoft.OpenApi.Extensions;
using SearchRank.Application.SearchEngine.Queries;

namespace SearchRank.Presentation.Responses;

public record SearchResponse(string Keyword, string TargetUrl, List<SearchEngineResponse> Results);

public record SearchEngineResponse(string SearchEngine, int? Rank);

public static class SearchResponseExtensions
{
    public static SearchResponse ToResponse(this SearchEngineQuery.ResultModel input)
    {
        return new SearchResponse(input.Keyword, input.TargetUrl, input.Results.Select(r => new SearchEngineResponse(r.Type.GetDisplayName(), r.Rank)).ToList());
    }
}