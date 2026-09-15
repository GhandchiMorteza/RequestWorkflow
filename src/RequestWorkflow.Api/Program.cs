using RequestWorkflow.Api.Authentication;
using RequestWorkflow.Api.ExceptionHandling;
using RequestWorkflow.Application;
using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Requests.Routing;
using RequestWorkflow.Infrastructure;
using RequestWorkflow.Infrastructure.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Application configuration
var requestRoutingOptions = builder.Configuration
    .GetSection(RequestRoutingOptions.SectionName)
    .Get<RequestRoutingOptions>()
    ?? throw new InvalidOperationException(
        $"Configuration section '{RequestRoutingOptions.SectionName}' is missing.");

// Application layer
builder.Services.AddApplication(requestRoutingOptions);

// Infrastructure layer
builder.Services.AddInfrastructure(
    builder.Configuration);

// Current authenticated user
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

// Authorization
builder.Services.AddAuthorization();

// Controllers + JSON serialization
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// Standard API error responses
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Development-only setup
if (app.Environment.IsDevelopment())
{
    var identitySeedOptions = builder.Configuration
        .GetSection(IdentitySeedOptions.SectionName)
        .Get<IdentitySeedOptions>()
        ?? throw new InvalidOperationException(
            $"Configuration section '{IdentitySeedOptions.SectionName}' is missing.");

    using var scope = app.Services.CreateScope();

    var seeder = scope.ServiceProvider
        .GetRequiredService<IdentitySeeder>();

    await seeder.SeedAsync(identitySeedOptions);

    app.MapOpenApi();
}

// Error handling
app.UseExceptionHandler();
app.UseStatusCodePages();

// HTTPS
app.UseHttpsRedirection();

// Security
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();

app.Run();
