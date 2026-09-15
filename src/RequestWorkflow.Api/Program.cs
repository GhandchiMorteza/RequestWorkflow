using RequestWorkflow.Application.Authentication;
using RequestWorkflow.Application.Requests.Routing;
using RequestWorkflow.Infrastructure;
using RequestWorkflow.Infrastructure.Identity;
using RequestWorkflow.Api.Authentication;
using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Requests.Create;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AuthenticationService>();

var requestRoutingOptions = builder.Configuration
    .GetSection(RequestRoutingOptions.SectionName)
    .Get<RequestRoutingOptions>()
    ?? throw new InvalidOperationException(
        $"Configuration section '{RequestRoutingOptions.SectionName}' is missing.");

if (requestRoutingOptions.ManagerApprovalMaxAmount <= 0)
{
    throw new InvalidOperationException(
        "RequestRouting:ManagerApprovalMaxAmount must be greater than zero.");
}

builder.Services.AddSingleton(requestRoutingOptions);

builder.Services.AddScoped<
    IRequestRoutingService,
    RequestRoutingService>();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<CreateRequestService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var identitySeedOptions = builder.Configuration
        .GetSection(IdentitySeedOptions.SectionName)
        .Get<IdentitySeedOptions>()
        ?? throw new InvalidOperationException(
            $"Configuration section '{IdentitySeedOptions.SectionName}' is missing.");

    using var scope = app.Services.CreateScope();

    var seeder =
        scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

    await seeder.SeedAsync(identitySeedOptions);

    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
