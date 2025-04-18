using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Services;
using System.Net;

namespace SearchRank.UnitTest.Infrastructure
{
    [TestClass]
    public class SearchEngineServiceTests
    {
        private Mock<IHttpClientFactory> _httpFactoryMock = null!;
        private Mock<IHtmlFinder> _htmlFinderMock = null!;
        private Mock<ILogger<SearchEngineService>> _loggerMock = null!;
        private SearchEngineService _searchEngineService = null!;

        [TestInitialize]
        public void Setup()
        {
            _httpFactoryMock = new Mock<IHttpClientFactory>();
            _htmlFinderMock = new Mock<IHtmlFinder>();
            _loggerMock = new Mock<ILogger<SearchEngineService>>();

            _searchEngineService = new SearchEngineService(
                _httpFactoryMock.Object,
                _htmlFinderMock.Object,
                _loggerMock.Object
            );
        }

        private HttpClient CreateHttpClientReturning(string content, HttpStatusCode code = HttpStatusCode.OK)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(code)
                {
                    Content = new StringContent(content)
                });

            return new HttpClient(handlerMock.Object);
        }

        [TestMethod]
        public async Task SearchGoogleAsync_WhenContentEmpty_ReturnsEmptyCollection()
        {
            // Arrange
            var keyword = "foo";
            var url = "https://target/";
            var http = CreateHttpClientReturning(string.Empty, HttpStatusCode.OK);
            _httpFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(http);

            // Act
            var result = await _searchEngineService.SearchGoogleAsync(keyword, url);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task SearchGoogleAsync_WhenContentNotEmpty_InvokesFinder()
        {
            // Arrange
            var keyword = "bar";
            var url = "https://target/";
            var fakeHtml = "<html>...</html>";
            var positions = new List<int> { 2, 5, 9 };
            var http = CreateHttpClientReturning(fakeHtml, HttpStatusCode.OK);
            _httpFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(http);
            _htmlFinderMock
                .Setup(h => h.FindUrlPositionsForGoogle(fakeHtml, url))
                .Returns(positions);

            // Act
            var result = await _searchEngineService.SearchGoogleAsync(keyword, url);

            // Assert
            result.Should().BeEquivalentTo(positions);
        }

        [TestMethod]
        public async Task SearchBingAsync_WhenEveryPageEmpty_SkipsAllPages()
        {
            // Arrange
            var keyword = "kw";
            var url = "https://target/";
            var pageSize = CommonConstant.MaxTotalResults; // totalPages = 1

            var http = CreateHttpClientReturning(string.Empty, HttpStatusCode.OK);
            _httpFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(http);

            // Act
            var result = await _searchEngineService.SearchBingAsync(keyword, url, pageSize);

            // Assert
            result.Should().BeEmpty();
            _htmlFinderMock.Verify(
                h => h.FindUrlPositionsForBing(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never
            );
        }

        [TestMethod]
        public async Task SearchBingAsync_WhenPagesHaveResults_AggregatesWithOffset()
        {
            // Arrange
            var keyword = "test";
            var url = "https://target/";
            var resultsPerPage = CommonConstant.MaxTotalResults / 2; // totalPages = 2
            var htmlPage1 = "<page1/>";
            var htmlPage2 = "<page2/>";
            var posPage1 = new[] { 0, 2 };
            var posPage2 = new[] { 1 };
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            handlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(htmlPage1)
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(htmlPage2)
                });
            _httpFactoryMock
                  .SetupSequence(f => f.CreateClient(It.IsAny<string>()))
                  .Returns(() => new HttpClient(handlerMock.Object))
                  .Returns(() => new HttpClient(handlerMock.Object));
            _htmlFinderMock
                .Setup(h => h.FindUrlPositionsForBing(htmlPage1, url))
                .Returns(posPage1);
            _htmlFinderMock
                .Setup(h => h.FindUrlPositionsForBing(htmlPage2, url))
                .Returns(posPage2);

            // Act
            var result = await _searchEngineService.SearchBingAsync(keyword, url, resultsPerPage);

            // Assert
            var expected = new List<int> { 0, 2, 1 + resultsPerPage };
            result.Should().BeEquivalentTo(expected);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
