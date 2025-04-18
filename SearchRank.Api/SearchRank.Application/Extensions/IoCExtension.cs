using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace SearchRank.Application.Extensions;

public static class IoCExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });
    }
}