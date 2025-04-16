using SearchRank.Application.User.Commands;

namespace SearchRank.Presentation.Requests;

public record LoginRequest(string Email, string Password);

public static class LoginRequestExtensions
{
    public static LoginUserCommand ToCommand(this LoginRequest input)
    {
        return new LoginUserCommand(input.Email, input.Password);
    }
}