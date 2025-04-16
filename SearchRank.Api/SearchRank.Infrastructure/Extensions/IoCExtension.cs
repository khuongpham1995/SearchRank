using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SearchRank.Domain.Interfaces;
using SearchRank.Infrastructure.Persistence;
using SearchRank.Infrastructure.Persistence.Repositories;
using SearchRank.Infrastructure.Services;

namespace SearchRank.Infrastructure.Extensions;

public static class IoCExtension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContextPool<AppDbContext>(options => { options.UseInMemoryDatabase("TestingMemoryDb"); });
        services.AddMemoryCache();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQueryLogRepository, QueryLogRepository>();
        services.AddTransient<IJwtGenerator, JwtGenerator>();
        services.AddTransient<IPasswordGenerator, PasswordGenerator>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddHttpClient<ISearchEngineService, SearchEngineService>();
    }
}