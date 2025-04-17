using MediatR;
using OneOf;
using SearchRank.Domain.Enums;
using SearchRank.Domain.Models;

namespace SearchRank.Application.SearchEngine.Queries;

public class SearchEngineQuery(string keyword, string targetUrl, SearchEngineType type) : IRequest<OneOf<SearchEngineQuery.ResultModel, Error>>
{
    public string Keyword { get; set; } = keyword;
    public string TargetUrl { get; set; } = targetUrl;
    public SearchEngineType SearchEngineType { get; set; } = type;

    public class ResultModel
    {
        public required string Keyword { get; set; }
        public required string TargetUrl { get; set; }
        public required Domain.Models.SearchEngine Result { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}