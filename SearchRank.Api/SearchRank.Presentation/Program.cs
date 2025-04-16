using System.Text;
using System.Threading.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SearchRank.Application.Extensions;
using SearchRank.Infrastructure.Extensions;
using SearchRank.Infrastructure.Persistence;
using SearchRank.Presentation;
using SearchRank.Presentation.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

// Add this before Swagger
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
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddRateLimiter(rl => rl
    .AddFixedWindowLimiter(ApplicationConstants.FixedRateLimitingPolicy, options =>
    {
        options.PermitLimit = Convert.ToInt32(builder.Configuration["RateLimiter:PermitLimit"]);
        options.Window = TimeSpan.FromSeconds(Convert.ToInt32(builder.Configuration["RateLimiter:FixedWindowSecond"]));
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = Convert.ToInt32(builder.Configuration["RateLimiter:QueueLimit"]);
    }).RejectionStatusCode = StatusCodes.Status429TooManyRequests);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
// Initialize the database (if applicable)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(context);
}

var group = app.MapGroup("/api");

group.MapPost("/token", async (LoginRequest loginRequest, ISender mediator) =>
{
    var response = await mediator.Send(loginRequest.ToCommand());
    return response.Match(token => Results.Ok(new { Token = token }), error => Results.BadRequest(error.Message));
}).WithTags("Bearer Token").RequireRateLimiting(ApplicationConstants.FixedRateLimitingPolicy);

group.MapPost("/searching", async (SearchRequest request, IMediator mediator) =>
{
    var response = await mediator.Send(request.ToQuery());
    return response.Match(Results.Ok, _ => Results.NoContent());
})
.WithTags("Search Rank")
.RequireAuthorization()
.RequireRateLimiting(ApplicationConstants.FixedRateLimitingPolicy);;

app.Run();