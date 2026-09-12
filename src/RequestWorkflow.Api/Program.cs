using RequestWorkflow.Application.Requests.Routing;
using RequestWorkflow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
