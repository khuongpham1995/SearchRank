using Microsoft.EntityFrameworkCore;
using SearchRank.Domain.Entities;
using SearchRank.Domain.Interfaces;

namespace SearchRank.Infrastructure.Persistence.Repositories;

public class QueryLogRepository(AppDbContext dbContext) : IQueryLogRepository
{
    public async Task AddAsync(SearchQuery entity)
    {
        await dbContext.Set<SearchQuery>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task<List<SearchQuery>> GetByUserIdAsync(Guid userId)
    {
        return await dbContext.Set<SearchQuery>().AsNoTracking()
            .Where(q => q.UserId == userId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }
}