using System.ComponentModel.DataAnnotations;

namespace SearchRank.Domain.Entities;

public class User
{
    public User()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = [];
}