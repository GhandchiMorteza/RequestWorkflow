using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Abstractions.Persistence;
using RequestWorkflow.Domain.Requests;
using RequestEntity = RequestWorkflow.Domain.Requests.Request;

namespace RequestWorkflow.Application.Requests.Review;

public sealed class ReviewRequestService
{
    private readonly IGenericRepository<RequestEntity> _requestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReviewRequestService(
        IGenericRepository<RequestEntity> requestRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(requestRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(currentUserService);

        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public Task<RequestReviewResult> ApproveAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return ReviewAsync(
            requestId,
            approve: true,
            cancellationToken);
    }

    public Task<RequestReviewResult> RejectAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return ReviewAsync(
            requestId,
            approve: false,
            cancellationToken);
    }

    private async Task<RequestReviewResult> ReviewAsync(
        Guid requestId,
        bool approve,
        CancellationToken cancellationToken)
    {
        var request = await _requestRepository.GetByIdAsync(
            requestId,
            cancellationToken);

        if (request is null)
        {
            return new RequestReviewResult(
                RequestReviewOutcome.NotFound,
                requestId);
        }

        if (!CanReview(request.AssignedRole))
        {
            return new RequestReviewResult(
                RequestReviewOutcome.Forbidden,
                requestId);
        }

        if (request.Status != RequestStatus.Pending)
        {
            return new RequestReviewResult(
                RequestReviewOutcome.Conflict,
                requestId,
                request.Status);
        }

        if (approve)
        {
            request.Approve();
        }
        else
        {
            request.Reject();
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new RequestReviewResult(
            RequestReviewOutcome.Success,
            request.Id,
            request.Status);
    }

    private bool CanReview(ApprovalRole assignedRole)
    {
        var roles = _currentUserService.Roles;

        return assignedRole switch
        {
            ApprovalRole.Manager =>
                roles.Contains(
                    ApplicationRoles.Manager,
                    StringComparer.OrdinalIgnoreCase),

            ApprovalRole.Finance =>
                roles.Contains(
                    ApplicationRoles.Finance,
                    StringComparer.OrdinalIgnoreCase),

            _ => false
        };
    }
}
