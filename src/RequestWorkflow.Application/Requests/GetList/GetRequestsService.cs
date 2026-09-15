using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Abstractions.Persistence;
using RequestWorkflow.Domain.Requests;
using RequestEntity = RequestWorkflow.Domain.Requests.Request;

namespace RequestWorkflow.Application.Requests.GetList;

public sealed class GetRequestsService
{
    private readonly IGenericRepository<RequestEntity> _requestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRequestsService(
        IGenericRepository<RequestEntity> requestRepository,
        ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(requestRepository);
        ArgumentNullException.ThrowIfNull(currentUserService);

        _requestRepository = requestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<RequestListItem>> GetAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;
        var roles = _currentUserService.Roles;

        var isManager = roles.Contains(
            ApplicationRoles.Manager,
            StringComparer.OrdinalIgnoreCase);

        var isFinance = roles.Contains(
            ApplicationRoles.Finance,
            StringComparer.OrdinalIgnoreCase);

        var requests = await _requestRepository.ListAsync(
            request =>
                request.CreatedByUserId == userId
                || (isManager &&
                    request.AssignedRole == ApprovalRole.Manager)
                || (isFinance &&
                    request.AssignedRole == ApprovalRole.Finance),
            cancellationToken);

        return [.. requests
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => new RequestListItem(
                request.Id,
                request.Title,
                request.Amount,
                request.Description,
                request.Status,
                request.CreatedByUserId,
                request.AssignedRole,
                request.CreatedAt,
                request.Metadata))];
    }
}
