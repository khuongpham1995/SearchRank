using SearchRank.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace SearchRank.Infrastructure.Services;

public partial class HtmlFinder : IHtmlFinder
{
    private const string GooglePattern = @"<div class=""BNeawe UPmit AP7Wnd.*?"">(.*?)<\/div>";
    private const string BingPattern = @"<li class=""b_algo.*?"">(.*?)<\/li>";
    private const string CiteTagPattern =
        @"<div[^>]*class=[""']notranslate[^""']*[""'][^>]*>.*?<cite[^>]*>(.*?)<\/cite>";
    private const string RegexPattern = @"href=""(https?:\/\/[^""]+)""";

    public IReadOnlyCollection<int> FindUrlPositionsForGoogle(string htmlContent, string targetUrl)
    {
        var positions = new List<int>();
        var matches = GetRegexMatches(htmlContent, GooglePattern);
        if (matches.Count == 0) matches = GetRegexMatches(htmlContent, CiteTagPattern);
        targetUrl = NormalizeUrl(targetUrl);
        for (var i = 0; i < matches.Count; i++)
        {
            var extractedText = matches[i].Groups[1].Value.Trim();

            if (!string.IsNullOrWhiteSpace(extractedText) &&
                extractedText.Contains(targetUrl, StringComparison.OrdinalIgnoreCase)) positions.Add(i + 1);
        }

        return positions;
    }

    public IReadOnlyCollection<int> FindUrlPositionsForBing(string htmlContent, string targetUrl)
    {
        var positions = new List<int>();
        var matches = GetRegexMatches(htmlContent, BingPattern);

        targetUrl = NormalizeUrl(targetUrl);

        for (var i = 0; i < matches.Count; i++)
        {
            var extractedText = matches[i].Groups[1].Value.Trim();
            var extractedHref = ExtractHrefValue(extractedText);

            if (!string.IsNullOrWhiteSpace(extractedHref) &&
                extractedHref.Contains(targetUrl, StringComparison.OrdinalIgnoreCase)) positions.Add(i + 1);
        }

        return positions;
    }

    private static MatchCollection GetRegexMatches(string htmlContent, string regexPattern)
    {
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return regex.Matches(htmlContent);
    }

    private static string? ExtractHrefValue(string htmlContent)
    {
        var regex = ExtractHrefRegex();
        var match = regex.Match(htmlContent);
        return match is { Success: true, Groups.Count: > 1 } ? NormalizeUrl(match.Groups[1].Value) : null;
    }

    private static string NormalizeUrl(string url)
    {
        return string.IsNullOrWhiteSpace(url)
            ? string.Empty
            : url.Replace("https://", "").Replace("http://", "").TrimEnd('/').ToLower();
    }

    [GeneratedRegex(RegexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ExtractHrefRegex();
}