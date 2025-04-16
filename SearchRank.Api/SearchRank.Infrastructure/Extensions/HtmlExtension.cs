using SearchRank.Domain.Constants;
using System.Text.RegularExpressions;

namespace SearchRank.Infrastructure.Extensions
{
    internal static class HtmlExtension
    {
        public static List<int> FindUrlPositionsForGoogle(string htmlContent, string targetUrl)
        {
            var positions = new List<int>();
            var matches = GetRegexMatches(htmlContent, CommonConstant.GooglePattern);

            if (matches.Count != 0) targetUrl = NormalizeUrl(targetUrl);
            else matches = GetRegexMatches(htmlContent, CommonConstant.CiteTagPattern);

            for (int i = 0; i < matches.Count; i++)
            {
                var extractedText = matches[i].Groups[1].Value.Trim();

                if (!string.IsNullOrWhiteSpace(extractedText) && extractedText.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(i + 1);
                }
            }

            return positions;
        }

        public static List<int> FindUrlPositionsForBing(string htmlContent, string targetUrl)
        {
            var positions = new List<int>();
            var matches = GetRegexMatches(htmlContent, CommonConstant.BingPattern);

            targetUrl = NormalizeUrl(targetUrl);

            for (int i = 0; i < matches.Count; i++)
            {
                var extractedText = matches[i].Groups[1].Value.Trim();
                var extractedHref = ExtractHrefValue(extractedText);

                if (!string.IsNullOrWhiteSpace(extractedHref) && extractedHref.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(i + 1);
                }
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
            var regexPattern = @"href=""(https?:\/\/[^""]+)""";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regex.Match(htmlContent);

            if (match.Success && match.Groups.Count > 1)
            {
                return NormalizeUrl(match.Groups[1].Value);
            }

            return null;
        }

        private static string NormalizeUrl(string url)
        {
            return string.IsNullOrWhiteSpace(url) ? string.Empty : url.Replace("https://", "").Replace("http://", "").Trim().ToLowerInvariant();
        }
    }
}
