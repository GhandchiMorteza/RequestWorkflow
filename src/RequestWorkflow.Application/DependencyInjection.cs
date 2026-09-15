using Microsoft.Extensions.DependencyInjection;
using RequestWorkflow.Application.Authentication;
using RequestWorkflow.Application.Requests.Create;
using RequestWorkflow.Application.Requests.GetList;
using RequestWorkflow.Application.Requests.Review;
using RequestWorkflow.Application.Requests.Routing;

namespace RequestWorkflow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        RequestRoutingOptions requestRoutingOptions)
    {
        ArgumentNullException.ThrowIfNull(requestRoutingOptions);

        if (requestRoutingOptions.ManagerApprovalMaxAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestRoutingOptions),
                "Manager approval max amount must be greater than zero.");
        }

        services.AddSingleton(requestRoutingOptions);

        services.AddScoped<
            IRequestRoutingService,
            RequestRoutingService>();

        services.AddScoped<AuthenticationService>();

        services.AddScoped<CreateRequestService>();
        services.AddScoped<GetRequestsService>();
        services.AddScoped<ReviewRequestService>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
