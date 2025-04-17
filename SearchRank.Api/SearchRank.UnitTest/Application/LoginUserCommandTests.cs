using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SearchRank.Application.User.Commands;
using SearchRank.Domain.Entities;
using SearchRank.Domain.Interfaces;

namespace SearchRank.UnitTest.Application;

[TestClass]
public class LoginUserCommandTests
{
    private Mock<IJwtGenerator> _jwtGeneratorMock = null!;
    private Mock<ILogger<LoginUserCommandHandler>> _loggerMock = null!;
    private Mock<IPasswordGenerator> _passwordGeneratorMock = null!;
    private ServiceProvider _serviceProvider = null!;
    private Mock<IUserRepository> _userRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        _userRepository = new Mock<IUserRepository>();
        _passwordGeneratorMock = new Mock<IPasswordGenerator>();
        _jwtGeneratorMock = new Mock<IJwtGenerator>();
        _loggerMock = new Mock<ILogger<LoginUserCommandHandler>>();
        services.AddSingleton(_userRepository.Object);
        services.AddSingleton(_passwordGeneratorMock.Object);
        services.AddSingleton(_jwtGeneratorMock.Object);
        services.AddSingleton(_loggerMock.Object);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly));
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task Login_ShouldReturnToken_WhenInputValid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        var email = "test@example.com";
        var plainPassword = "Password123!";
        var hashedPassword = "hashedpassword";
        var token = "some_jwt_token";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hashedPassword
        };

        _userRepository.Setup(repo => repo.GetUserByEmailAsync(email))
            .ReturnsAsync(user);
        _passwordGeneratorMock.Setup(ps => ps.ValidatePassword(plainPassword, hashedPassword))
            .Returns(true);
        _jwtGeneratorMock.Setup(jwt => jwt.GenerateToken(user))
            .Returns(token);

        // Act
        var result = await mediator.Send(new LoginUserCommand { Email = email, Password = plainPassword });

        // Assert
        result.IsT0.Should().BeTrue("because the login should be successful");
        result.AsT0.Token.Should().Be(token, "because the token should match the expected value");
    }

    [TestMethod]
    public async Task Login_ShouldReturnError_WhenEmailInvalid()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var command = new LoginUserCommand { Email = "aaa", Password = "Password123!" };

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsT1.Should().BeTrue("because the email is invalid");
    }

    [TestMethod]
    public async Task Login_ShouldReturnError_WhenInvalidPassword()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        var email = "test@example.com";
        var plainPassword = "Password123!";
        var hashedPassword = "hashedpassword";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hashedPassword
        };

        _userRepository.Setup(repo => repo.GetUserByEmailAsync(email))
            .ReturnsAsync(user);
        _passwordGeneratorMock.Setup(ps => ps.ValidatePassword(plainPassword, hashedPassword))
            .Returns(false);

        // Act
        var result = await mediator.Send(new LoginUserCommand { Email = email, Password = plainPassword });

        // Assert
        result.IsT1.Should().BeTrue("because the password is invalid");
    }
}