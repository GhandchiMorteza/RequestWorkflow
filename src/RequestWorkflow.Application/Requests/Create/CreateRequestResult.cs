using RequestWorkflow.Domain.Requests;

namespace RequestWorkflow.Application.Requests.Create;

public sealed record CreateRequestResult(
    Guid Id,
    string Title,
    decimal Amount,
    RequestStatus Status,
    ApprovalRole AssignedRole,
    DateTimeOffset CreatedAt);
