namespace RequestWorkflow.Application.Authentication;

public sealed record AuthenticationResponse(
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Roles,
    string AccessToken,
    DateTimeOffset ExpiresAt);
