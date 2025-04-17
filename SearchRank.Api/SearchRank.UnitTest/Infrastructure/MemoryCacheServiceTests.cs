using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SearchRank.Infrastructure.Services;

namespace SearchRank.UnitTest.Infrastructure;

[TestClass]
public class MemoryCacheServiceTests
{
    private MemoryCacheService _cacheService = null!;
    private Mock<IMemoryCache> _memoryCache = null!;

    [TestInitialize]
    public void Setup()
    {
        _memoryCache = new Mock<IMemoryCache>();
        _cacheService = new MemoryCacheService(_memoryCache.Object);
    }

    [TestMethod]
    public void TryGetValue_ShouldReturnTrue_AndOutTheValueWhenCacheHit()
    {
        // Arrange
        var key = "my-key";
        var value = "hello";
        object? boxed = value;
        _memoryCache.Setup(c => c.TryGetValue(key, out boxed)).Returns(true);

        // Act
        var got = _cacheService.TryGetValue<string>(key, out var result);

        // Assert
        got.Should().BeTrue();
        result.Should().Be(value);
    }

    [TestMethod]
    public void Set_ShouldCreateEntry_WithCorrectValueAndExpiration()
    {
        // Arrange
        var key = "another-key";
        var value = 123;
        var expiry = TimeSpan.FromMinutes(5);
        var entryMock = new Mock<ICacheEntry>();
        entryMock.SetupAllProperties();
        _memoryCache.Setup(c => c.CreateEntry(key)).Returns(entryMock.Object);

        // Act
        _cacheService.Set(key, value, expiry);

        // Assert
        entryMock.VerifySet(e => e.AbsoluteExpirationRelativeToNow = expiry, Times.Once);
        entryMock.VerifySet(e => e.Value = value, Times.Once);
        entryMock.Verify(e => e.Dispose(), Times.Once);
    }

    [TestMethod]
    public void Remove_ShouldCallUnderlyingCacheRemove()
    {
        // Arrange
        var key = "to-remove";

        // Act
        _cacheService.Remove(key);

        // Assert
        _memoryCache.Verify(c => c.Remove(key), Times.Once);
    }
}