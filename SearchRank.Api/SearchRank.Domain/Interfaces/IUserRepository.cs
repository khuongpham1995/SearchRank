using SearchRank.Domain.Entities;

namespace SearchRank.Domain.Interfaces;

public interface IUserRepository
{
    /// <summary>
    ///     Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to find.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetUserByEmailAsync(string email);
}