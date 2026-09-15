using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Abstractions.Persistence;
using RequestWorkflow.Application.Requests.Routing;
using RequestEntity = RequestWorkflow.Domain.Requests.Request;

namespace RequestWorkflow.Application.Requests.Create;

public sealed class CreateRequestService
{
    private readonly IGenericRepository<RequestEntity> _requestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRequestRoutingService _routingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public CreateRequestService(
        IGenericRepository<RequestEntity> requestRepository,
        IUnitOfWork unitOfWork,
        IRequestRoutingService routingService,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(requestRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(routingService);
        ArgumentNullException.ThrowIfNull(currentUserService);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _routingService = routingService;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async Task<CreateRequestResult> CreateAsync(
        CreateRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var assignedRole =
            _routingService.DetermineApprovalRole(command.Amount);

        var request = RequestEntity.Create(
            command.Title,
            command.Amount,
            command.Description,
            _currentUserService.UserId,
            assignedRole,
            _timeProvider.GetUtcNow(),
            command.Metadata);

        await _requestRepository.AddAsync(
            request,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateRequestResult(
            request.Id,
            request.Title,
            request.Amount,
            request.Status,
            request.AssignedRole,
            request.CreatedAt);
    }
}
