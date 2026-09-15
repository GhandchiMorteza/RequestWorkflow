namespace RequestWorkflow.Application.Requests.Review;

public enum RequestReviewOutcome
{
    Success = 1,
    NotFound = 2,
    Forbidden = 3,
    Conflict = 4
}
