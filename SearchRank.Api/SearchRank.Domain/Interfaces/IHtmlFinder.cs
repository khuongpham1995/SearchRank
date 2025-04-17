namespace SearchRank.Domain.Interfaces;

public interface IHtmlFinder
{
    IReadOnlyCollection<int> FindUrlPositionsForBing(string html, string url);
    IReadOnlyCollection<int> FindUrlPositionsForGoogle(string html, string url);
}