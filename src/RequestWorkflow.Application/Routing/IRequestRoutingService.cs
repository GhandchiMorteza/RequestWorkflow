using RequestWorkflow.Domain.Requests;

namespace RequestWorkflow.Application.Requests.Routing;

public interface IRequestRoutingService
{
    ApprovalRole DetermineApprovalRole(decimal amount);
}
