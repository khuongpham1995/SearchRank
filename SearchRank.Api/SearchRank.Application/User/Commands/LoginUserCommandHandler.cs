using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using SearchRank.Domain.Interfaces;
using SearchRank.Domain.Models;

namespace SearchRank.Application.User.Commands;

public class LoginUserCommandHandler(
    IUserRepository repository,
    IJwtGenerator jwtGenerator,
    IPasswordGenerator passwordGenerator,
    ILogger<LoginUserCommandHandler> logger)
    : IRequestHandler<LoginUserCommand, OneOf<LoginUserCommand.ResultModel, Error>>
{
    public async Task<OneOf<LoginUserCommand.ResultModel, Error>> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling LoginUserCommand for Email: {Email}", request.Email);
        var user = await repository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            logger.LogWarning("Login attempt failed: No user found with Email: {Email}", request.Email);
            return new Error("Invalid password or user is not existing.");
        }

        if (!passwordGenerator.ValidatePassword(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login attempt failed: Incorrect password for Email: {Email}", request.Email);
            return new Error("Invalid password or user is not existing.");
        }

        logger.LogInformation("User {Email} successfully authenticated", request.Email);
        var token = jwtGenerator.GenerateToken(user);
        logger.LogInformation("JWT token generated for user: {Email}", request.Email);

        return new LoginUserCommand.ResultModel { Token = token };
    }
}