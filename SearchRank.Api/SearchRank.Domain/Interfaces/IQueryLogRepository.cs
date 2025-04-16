using SearchRank.Domain.Entities;

namespace SearchRank.Domain.Interfaces;

public interface IQueryLogRepository
{
    /// <summary>
    /// Adds a new search query log to the database asynchronously.
    /// </summary>
    /// <param name="searchQuery">The search query entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(SearchQuery searchQuery);
    
    /// <summary>
    /// Retrieves search query logs for a specific user ordered by the creation date descending.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>A collection of search query logs.</returns>
    Task<List<SearchQuery>> GetByUserIdAsync(Guid userId);
}