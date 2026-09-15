using RequestWorkflow.Api.Authentication;
using RequestWorkflow.Api.ExceptionHandling;
using RequestWorkflow.Api.OpenApi;
using RequestWorkflow.Application;
using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Requests.Routing;
using RequestWorkflow.Infrastructure;
using RequestWorkflow.Infrastructure.Identity;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var requestRoutingOptions = builder.Configuration
    .GetSection(RequestRoutingOptions.SectionName)
    .Get<RequestRoutingOptions>()
    ?? throw new InvalidOperationException(
        $"Configuration section '{RequestRoutingOptions.SectionName}' is missing.");

builder.Services.AddApplication(requestRoutingOptions);

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddAuthorization();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();

    options.AddOperationTransformer<
        AuthOperationTransformer>();
});

var app = builder.Build();

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
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
