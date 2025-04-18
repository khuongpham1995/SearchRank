using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SearchRank.Application.SearchEngine.Queries;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Enums;
using SearchRank.Domain.Interfaces;

namespace SearchRank.UnitTest.Application;

[TestClass]
public class SearchEngineQueryTests
{
    private Mock<ICacheService> _cacheService = null!;
    private Mock<ILogger<SearchEngineQueryHandler>> _loggerMock = null!;
    private Mock<ISearchEngineService> _searchEngineService = null!;
    private ServiceProvider _serviceProvider = null!;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        _searchEngineService = new Mock<ISearchEngineService>();
        _cacheService = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<SearchEngineQueryHandler>>();
        services.AddSingleton(_searchEngineService.Object);
        services.AddSingleton(_cacheService.Object);
        services.AddSingleton(_loggerMock.Object);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SearchEngineQuery).Assembly));
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task Search_ShouldReturnError_WhenTargetUrlIsWhitespace()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var request = new SearchEngineQuery(
            "anything",
            "   ",
            SearchEngineType.Google
        );

        // Act
        var result = await mediator.Send(request, It.IsAny<CancellationToken>());

        // Assert
        result.IsT1.Should().BeTrue("expected an Error result");
        result.AsT1.Message.Should().Be("TargetUrl is required");
    }

    [TestMethod]
    public async Task Search_ShouldReturnError_WhenKeywordIsNull()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var request = new SearchEngineQuery(
            null!,
            "http://example.com",
            SearchEngineType.Bing
        );

        // Act
        var result = await mediator.Send(request, It.IsAny<CancellationToken>());

        // Assert
        result.IsT1.Should().BeTrue("expected an Error result");
        result.AsT1.Message.Should().Be("Keyword is required");
    }

    [TestMethod]
    public async Task Search_ShouldReturnError_WhenKeywordIsNullOrEmpty()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var request = new SearchEngineQuery(
            string.Empty,
            "http://example.com",
            SearchEngineType.Bing
        );

        // Act
        var result = await mediator.Send(request, It.IsAny<CancellationToken>());

        // Assert
        result.IsT1.Should().BeTrue("expected an Error result");
        result.AsT1.Message.Should().Be("Keyword is required");
    }

    [TestMethod]
    public async Task Search_BingCacheHit_ShouldReturnCachedRank_AndNotCallSearchService()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var request = new SearchEngineQuery("example", "http://example.com", SearchEngineType.Bing);
        var cachedList = new List<int> { 2, 4, 6 };
        _cacheService
            .Setup(x =>
                x.TryGetValue(
                    It.IsAny<string>(),
                    out It.Ref<IReadOnlyCollection<int>?>.IsAny
                )
            )
            .Callback((string _, out IReadOnlyCollection<int>? value) => { value = cachedList; })
            .Returns(true);

        // Act
        var result = await mediator.Send(request, It.IsAny<CancellationToken>());

        // Assert
        result.IsT0.Should().BeTrue("expected a successful ResultModel");
        var model = result.AsT0;
        model.Keyword.Should().Be(request.Keyword);
        model.TargetUrl.Should().Be(request.TargetUrl);
        model.Result.Rank.Should().Be("2, 4, 6");
        _cacheService.Verify(svc => svc.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [TestMethod]
    public async Task Search_GoogleCacheMiss_ShouldFetchRankAndCacheIt()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var keyword = "foo";
        var targetUrl = "http://bar.com";
        var request = new SearchEngineQuery(keyword, targetUrl, SearchEngineType.Google);
        var cacheKey = string.Format(CommonConstant.CacheKeyFormat, SearchEngineType.Google, keyword, targetUrl);
        _cacheService.Setup(x => x.TryGetValue(cacheKey, out It.Ref<IReadOnlyCollection<int>?>.IsAny)).Returns(false);
        var returnedList = new List<int> { 7, 8 };
        _searchEngineService.Setup(svc => svc.SearchGoogleAsync(keyword, targetUrl)).ReturnsAsync(returnedList);

        // Act
        var result = await mediator.Send(request, CancellationToken.None);

        // Assert
        result.IsT0.Should().BeTrue("expected a successful ResultModel");
        var model = result.AsT0;
        model.Result.Rank.Should().Be("7, 8");
        _cacheService.Verify(
            c => c.Set(
                cacheKey,
                It.Is<IReadOnlyCollection<int>>(col => col.Count == 2 && col.Contains(7) && col.Contains(8)),
                TimeSpan.FromMinutes(CommonConstant.CacheExpireMinute)
            ),
            Times.Once);
    }
}