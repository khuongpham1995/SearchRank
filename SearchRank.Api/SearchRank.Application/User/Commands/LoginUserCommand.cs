using MediatR;
using OneOf;
using SearchRank.Domain.Models;

namespace SearchRank.Application.User.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<OneOf<string, Error>>;