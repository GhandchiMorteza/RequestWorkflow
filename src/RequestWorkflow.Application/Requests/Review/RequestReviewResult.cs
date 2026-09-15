using RequestWorkflow.Domain.Requests;

namespace RequestWorkflow.Application.Requests.Review;

public sealed record RequestReviewResult(
    RequestReviewOutcome Outcome,
    Guid RequestId,
    RequestStatus? Status = null);
