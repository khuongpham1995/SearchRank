using Microsoft.EntityFrameworkCore;
using SearchRank.Domain.Entities;
using SearchRank.Domain.Interfaces;

namespace SearchRank.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email) => await dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(p => p.Email == email);
}