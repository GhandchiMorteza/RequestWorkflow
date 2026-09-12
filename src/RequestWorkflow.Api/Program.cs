using RequestWorkflow.Application.Authentication;
using RequestWorkflow.Application.Requests.Routing;
using RequestWorkflow.Infrastructure;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
