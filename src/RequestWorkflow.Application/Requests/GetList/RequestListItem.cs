using RequestWorkflow.Domain.Requests;
using System.Text.Json;

namespace RequestWorkflow.Application.Requests.GetList;

public sealed record RequestListItem(
    Guid Id,
    string Title,
    decimal Amount,
    string? Description,
    RequestStatus Status,
    Guid CreatedByUserId,
    ApprovalRole AssignedRole,
    DateTimeOffset CreatedAt,
    JsonElement? Metadata);
