using Microsoft.OpenApi.Extensions;
using SearchRank.Application.SearchEngine.Queries;

namespace SearchRank.Presentation.Responses;

public record SearchResponse(string Keyword, string TargetUrl, SearchEngineResponse Result);

public record SearchEngineResponse(string SearchEngine, string? Rank);

public static class SearchResponseExtensions
{
    public static SearchResponse ToResponse(this SearchEngineQuery.ResultModel input)
    {
        return new SearchResponse(input.Keyword, input.TargetUrl, 
            new SearchEngineResponse(input.Result.Type.GetDisplayName(), input.Result.Rank));
    }
}