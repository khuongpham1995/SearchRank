using SearchRank.Domain.Entities;

namespace SearchRank.Domain.Interfaces;

public interface IJwtGenerator
{
    /// <summary>
    /// Generates a signed JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token should be generated. The token typically includes claims based on the user's properties.</param>
    /// <returns>A signed JWT as a string, which can be used for authentication and authorization.</returns>
    string GenerateToken(User user);
}