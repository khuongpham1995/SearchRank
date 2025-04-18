using SearchRank.Domain.Interfaces;
using System.Security.Cryptography;

namespace SearchRank.Infrastructure.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;
        private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;

        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithm, KeySize);

            return $"{Iterations}." +
                   $"{Convert.ToBase64String(salt)}." +
                   $"{Convert.ToBase64String(key)}";
        }

        public bool ValidatePassword(string password, string hashedPassword)
        {
            try
            {
                var parts = hashedPassword.Split('.');
                if (parts.Length != 3)
                    return false;

                if (!int.TryParse(parts[0], out var iterations))
                    return false;

                var salt = Convert.FromBase64String(parts[1]);
                var key = Convert.FromBase64String(parts[2]);
                var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    _hashAlgorithm,
                    key.Length
                );

                return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
            }
            catch
            {
                return false;
            }
        }
    }
}