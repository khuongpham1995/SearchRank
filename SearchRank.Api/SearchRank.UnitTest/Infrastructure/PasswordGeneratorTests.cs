using FluentAssertions;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Services;

namespace SearchRank.UnitTest.Infrastructure;

[TestClass]
public class PasswordGeneratorTests
{
    private IPasswordGenerator _generator = null!;

    [TestInitialize]
    public void Setup()
    {
        _generator = new PasswordGenerator();
    }

    [TestMethod]
    public void HashPassword_ShouldReturnThreeDotSeparatedParts()
    {
        // Act
        var hashed = _generator.HashPassword("MyP@ssw0rd!");

        // Assert
        var parts = hashed.Split('.');
        parts.Should().HaveCount(3);
        parts[0].Should().Be("10000", "default iteration count should be present");
        parts[1].Should().NotBeNullOrWhiteSpace("salt should be base64");
        parts[2].Should().NotBeNullOrWhiteSpace("key should be base64");
    }

    [TestMethod]
    public void HashPassword_SameInput_YieldsDifferentHashes()
    {
        // Act
        var hash1 = _generator.HashPassword("MyP@ss123");
        var hash2 = _generator.HashPassword("MyP@ss123");

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [TestMethod]
    public void ValidatePassword_CorrectPassword_ShouldReturnsTrue()
    {
        // Arrange
        var password = "CorrectHorseBatteryStaple";
        var hashed = _generator.HashPassword(password);

        // Act
        var result = _generator.ValidatePassword(password, hashed);

        // Assert
        result.Should().BeTrue("the original password should validate against its hash");
    }

    [TestMethod]
    public void ValidatePassword_IncorrectPassword_ShouldReturnsFalse()
    {
        // Arrange
        var password = "P@ssword123";
        var wrongPass = "p@ssword123";
        var hashed = _generator.HashPassword(password);

        // Act
        var result = _generator.ValidatePassword(wrongPass, hashed);

        // Assert
        result.Should().BeFalse("a different password should not validate");
    }

    [TestMethod]
    public void ValidatePassword_MalformedHash_ReturnsFalse()
    {
        // Arrange
        var badHash1 = "notdots";
        var badHash2 = "1.2";          // too few parts
        var badHash3 = "a.b.c.d";      // too many parts
        var badHash4 = "abc.def.ghi";  // non‑base64

        // Act & Assert
        _generator.ValidatePassword("whatever", badHash1).Should().BeFalse();
        _generator.ValidatePassword("whatever", badHash2).Should().BeFalse();
        _generator.ValidatePassword("whatever", badHash3).Should().BeFalse();
        _generator.ValidatePassword("whatever", badHash4).Should().BeFalse();
    }
}