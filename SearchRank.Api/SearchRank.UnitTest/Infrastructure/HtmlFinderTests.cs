using FluentAssertions;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Services;

namespace SearchRank.UnitTest.Infrastructure;

[TestClass]
public class HtmlFinderTests
{
    private IHtmlFinder _finder = null!;

    [TestInitialize]
    public void Setup()
    {
        _finder = new HtmlFinder();
    }

    [TestMethod]
    public void FindUrlPositionsForGoogle_ShouldFindExactDivMatches()
    {
        // Arrange
        var rawUrl = "https://example.com/path/";
        var html = @"
                <div class=""BNeawe UPmit AP7Wnd"">Some other result</div>
                <div class=""BNeawe UPmit AP7Wnd"">
                   visit example.com/path/article
                </div>
                <div class=""BNeawe UPmit AP7Wnd"">example.com/path/</div>
            ";

        // Act
        var positions = _finder.FindUrlPositionsForGoogle(html, rawUrl);

        // Assert
        positions.Should().Equal([2, 3]);
    }

    [TestMethod]
    public void FindUrlPositionsForGoogle_ShouldFallbackToCiteTag_WhenNoDivMatches()
    {
        // Arrange
        var rawUrl = "http://test.com";
        var html = @"
                <div class=""notranslate"">
                  <cite>http://test.com/page1</cite>
                </div>
                <div class=""notranslate"">
                  <cite>other.com</cite>
                </div>
                <div class=""notranslate"">
                  <cite>https://test.com/page2</cite>
                </div>
            ";

        // Act
        var positions = _finder.FindUrlPositionsForGoogle(html, rawUrl);

        // Assert
        positions.Should().Equal([1, 3]);
    }

    [TestMethod]
    public void FindUrlPositionsForGoogle_ShouldReturnEmpty_WhenNothingMatches()
    {
        // Arrange
        var html = "<div class=\"BNeawe UPmit AP7Wnd\">no hits here</div>";

        // Act
        var positions = _finder.FindUrlPositionsForGoogle(html, "anything");

        // Assert
        positions.Should().BeEmpty();
    }

    [TestMethod]
    public void FindUrlPositionsForBing_ShouldExtractHrefAndMatchNormalizedUrls()
    {
        // Arrange
        var rawUrl = "https://mysite.com/";
        var html = @"
                <li class=""b_algo"">
                  <h2><a href=""https://mysite.com/page1"">Title1</a></h2>
                </li>
                <li class=""b_algo"">
                  <h2><a href=""http://other.com"">Title2</a></h2>
                </li>
                <li class=""b_algo"">
                  <h2><a href=""https://MYSITE.com/page2"">Title3</a></h2>
                </li>
            ";

        // Act
        var positions = _finder.FindUrlPositionsForBing(html, rawUrl);

        // Asser
        positions.Should().Equal([1, 3]);
    }

    [TestMethod]
    public void FindUrlPositionsForBing_ShouldReturnEmpty_WhenNoHrefMatches()
    {
        // Arrange
        var html = @"
                <li class=""b_algo""><div>No links here</div></li>
                <li class=""b_algo""><a href="""">Empty href</a></li>
            ";
        // Act
        var positions = _finder.FindUrlPositionsForBing(html, "irrelevant");

        // Assert
        positions.Should().BeEmpty();
    }
}