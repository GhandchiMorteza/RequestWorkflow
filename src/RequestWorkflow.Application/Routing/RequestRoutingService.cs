using RequestWorkflow.Domain.Requests;

namespace RequestWorkflow.Application.Requests.Routing;

public sealed class RequestRoutingService : IRequestRoutingService
{
    private readonly RequestRoutingOptions _options;

    public RequestRoutingService(RequestRoutingOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.ManagerApprovalMaxAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "Manager approval max amount must be greater than zero.");
        }

        _options = options;
    }

    public ApprovalRole DetermineApprovalRole(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero.");
        }

        return amount <= _options.ManagerApprovalMaxAmount
            ? ApprovalRole.Manager
            : ApprovalRole.Finance;
    }
}
