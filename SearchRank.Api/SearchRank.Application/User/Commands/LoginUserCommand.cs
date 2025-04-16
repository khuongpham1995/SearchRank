using MediatR;
using OneOf;
using SearchRank.Domain.Models;

namespace SearchRank.Application.User.Commands;

public class LoginUserCommand : IRequest<OneOf<LoginUserCommand.ResultModel, Error>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    public class ResultModel
    {
        public string Token { get; set; } = string.Empty;
    }
}