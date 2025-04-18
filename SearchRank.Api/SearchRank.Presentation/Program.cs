using System.Text;
using System.Threading.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SearchRank.Application.Extensions;
using SearchRank.Domain.Constants;
using SearchRank.Domain.Enums;
using SearchRank.Infrastructure.Extensions;
using SearchRank.Infrastructure.Persistence;
using SearchRank.Presentation;
using SearchRank.Presentation.Middlewares;
using SearchRank.Presentation.Requests;
using SearchRank.Presentation.Responses;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables().Build();
var appConfig = builder.Configuration.Get<AppConfig>()!;
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Search Rank API",
        Version = "v1"
    });
    options.CustomSchemaIds(type => type.ToString());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(appConfig.JwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = appConfig.JwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = appConfig.JwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddRateLimiter(rl => rl
    .AddFixedWindowLimiter(CommonConstant.FixedRateLimitingPolicy, options =>
    {
        options.PermitLimit = Convert.ToInt32(appConfig.RateLimiter.PermitLimit);
        options.Window = TimeSpan.FromSeconds(Convert.ToInt32(appConfig.RateLimiter.FixedWindowSecond));
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = Convert.ToInt32(appConfig.RateLimiter.QueueLimit);
    }).RejectionStatusCode = StatusCodes.Status429TooManyRequests);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CommonConstant.AllowAngularApp, policy =>
    {
        if (appConfig.CorsSettings.AllowedOrigins.Length > 0) policy.WithOrigins(appConfig.CorsSettings.AllowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/search-rank.json", "v1"); });
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors(CommonConstant.AllowAngularApp);

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(context);
}

var group = app.MapGroup(appConfig.ApiAction.Grouping);

group.MapPost(appConfig.ApiAction.Token, TokenEndpoint)
.WithTags("Bearer Token")
.Produces<LoginResponse>()
.Produces(StatusCodes.Status400BadRequest)
.RequireRateLimiting(CommonConstant.FixedRateLimitingPolicy);

group.MapGet(appConfig.ApiAction.BingRank, BingEndpoint)
.WithTags("Search Engine")
.Produces<SearchResponse>()
.Produces(StatusCodes.Status204NoContent)
.RequireAuthorization()
.RequireRateLimiting(CommonConstant.FixedRateLimitingPolicy);

group.MapGet(appConfig.ApiAction.GoogleRank, GoogleEndpoint)
.WithTags("Search Engine")
.Produces<SearchResponse>()
.Produces(StatusCodes.Status204NoContent)
.RequireAuthorization()
.RequireRateLimiting(CommonConstant.FixedRateLimitingPolicy);

group.MapPost(appConfig.ApiAction.Log, LogEndpoint)
.WithTags("Server Log")
.Produces<object>(StatusCodes.Status200OK)
.Produces<string>(StatusCodes.Status400BadRequest)
.RequireAuthorization()
.RequireRateLimiting(CommonConstant.FixedRateLimitingPolicy);

app.Run();

static async Task<IResult> TokenEndpoint(
    LoginRequest loginRequest,
    ISender sender)
{
    var response = await sender.Send(loginRequest.ToCommand());
    return response.Match(
      ok => Results.Ok(ok.ToResponse()),
      error => Results.BadRequest(error.Message)
    );
}

static async Task<IResult> BingEndpoint(
    [AsParameters] SearchRequest request,
    ISender sender)
{
    var response = await sender.Send(request.ToQuery(SearchEngineType.Bing));
    return response.Match(
      ok => Results.Ok(ok.ToResponse()),
      _ => Results.NoContent()
    );
}

static async Task<IResult> GoogleEndpoint(
    [AsParameters] SearchRequest request,
    ISender sender)
{
    var response = await sender.Send(request.ToQuery(SearchEngineType.Google));
    return response.Match(
      ok => Results.Ok(ok.ToResponse()),
      _ => Results.NoContent()
    );
}

static IResult LogEndpoint(
    ServerLogRequest request,
    ILogger<Program> logger)
{
    if (string.IsNullOrWhiteSpace(request.Level) ||
        string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest("Invalid request.");
    }

    switch (request.Level.ToLower())
    {
        case "info":
            logger.LogInformation(request.Message);
            break;
        case "warn":
            logger.LogWarning(request.Message);
            break;
        case "error":
            logger.LogError(request.Message);
            break;
        default:
            logger.LogWarning("Unknown log level, logging: {Message}", request.Message);
            return Results.BadRequest($"Unknown log {request.Level}, logging: {request.Message}");
    }

    return Results.Ok(new { message = "Log recorded successfully" });
}