using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SearchRank.Domain.Entities;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SearchRank.UnitTest.Infrastructure;

[TestClass]
public class JwtGeneratorTests
{
    private const string SecretKey = "super-secret-key-which-is-long-enough";
    private const string Issuer = "my-api";
    private const string Audience = "my-clients";
    private IJwtGenerator _generator = null!;

    [TestInitialize]
    public void Setup()
    {
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
            .Setup(c => c["JwtSettings:SecretKey"])
            .Returns(SecretKey);
        configurationMock
            .Setup(c => c["JwtSettings:Issuer"])
            .Returns(Issuer);
        configurationMock
            .Setup(c => c["JwtSettings:Audience"])
            .Returns(Audience);
        _generator = new JwtGenerator(configurationMock.Object);
    }

    [TestMethod]
    public void GenerateToken_ShouldProduceValidJwt_WithCorrectClaims()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@example.com"
        };

        // Act
        var tokenString = _generator.GenerateToken(user);

        // Assert
        tokenString.Should().NotBeNullOrWhiteSpace("we expect a JWT string back");
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(tokenString).Should().BeTrue("the JwtSecurityTokenHandler must accept it");
        var jwt = handler.ReadJwtToken(tokenString);
        jwt.Issuer.Should().Be(Issuer, "the token must carry the configured issuer");
        jwt.Audiences.Should().ContainSingle().Which.Should().Be(Audience, "the token must carry the configured audience");
        jwt.Claims.Should().ContainSingle(c =>
                    c.Type == JwtRegisteredClaimNames.Sub &&
                    c.Value == user.Id.ToString(),
                "the 'sub' claim must match the user's ID");
        jwt.Claims.Should().ContainSingle(c =>
                    c.Type == JwtRegisteredClaimNames.Email &&
                    c.Value == user.Email,
                "the 'email' claim must match the user's email");
        jwt.ValidTo
            .Should().BeAfter(DateTime.UtcNow.AddMinutes(59), "expiry ~60 minutes from now")
            .And.BeBefore(DateTime.UtcNow.AddMinutes(61), "expiry window is roughly one hour");
    }
}