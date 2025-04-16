using MediatR;
using OneOf;
using SearchRank.Domain.Models;

namespace SearchRank.Application.SearchEngine.Queries;

public class SearchEngineQuery(string keyword, string targetUrl, Guid userId) : IRequest<OneOf<SearchEngineQuery.ResultModel, Error>>
{
    public string Keyword { get; set; } = keyword;
    public string TargetUrl { get; set; } = targetUrl;
    public Guid UserId { get; set; } = userId;

    public class ResultModel
    {
        public required string Keyword { get; set; }
        public required string TargetUrl { get; set; }
        public List<Domain.Models.SearchEngine> Results { get; set; } = [];
        public DateTimeOffset Timestamp { get; set; }
    }
}