using SearchRank.Application.User.Commands;

namespace SearchRank.Presentation.Responses;

public record LoginResponse(string Token);

public static class LoginResponseExtensions
{
    public static LoginResponse ToResponse(this LoginUserCommand.ResultModel input)
    {
        return new LoginResponse(input.Token);
    }
}