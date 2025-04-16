using Microsoft.EntityFrameworkCore;
using SearchRank.Domain.Entities;
using SearchRank.Infrastructure.Services;

namespace SearchRank.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        const string demoEmail = "test-user@gmail.com";
        var isExisting = await context.Set<User>().AnyAsync(u => u.Email.Equals(demoEmail));
        if (isExisting)
        {
            return;
        }
        
        var passwordService = new PasswordGenerator();
        await context.Set<User>().AddAsync(new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = demoEmail,
            PasswordHash = passwordService.HashPassword("Password123!")
        });
        await context.SaveChangesAsync();
    }
}