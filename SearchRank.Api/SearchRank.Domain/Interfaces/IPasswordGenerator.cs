namespace SearchRank.Domain.Interfaces;

public interface IPasswordGenerator
{
    /// <summary>
    ///     Generates a hashed password string from the plain text password.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>A hashed password in a composite format.</returns>
    string HashPassword(string password);

    /// <summary>
    ///     Validates a plain text password against a previously hashed password.
    /// </summary>
    /// <param name="password">The plain text password to validate.</param>
    /// <param name="hashedPassword">The hashed password for comparison.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    bool ValidatePassword(string password, string hashedPassword);
}