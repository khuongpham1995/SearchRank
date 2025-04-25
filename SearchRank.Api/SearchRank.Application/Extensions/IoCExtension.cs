using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SearchRank.Application.PipelineBehaviors;
using SearchRank.Application.SearchEngine.Queries;
using SearchRank.Application.User.Commands;
using System.Reflection;

public static class IoCExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.RegisterValidations();
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });

    }

    private static void RegisterValidations(this IServiceCollection services)
    {
        services.AddTransient<IValidator<LoginUserCommand>, LoginUserCommandValidator>();
        services.AddTransient<IValidator<SearchEngineQuery>, SearchEngineQueryValidator>();
    }
}