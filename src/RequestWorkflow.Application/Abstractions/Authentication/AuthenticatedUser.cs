namespace RequestWorkflow.Application.Abstractions.Authentication;

public sealed record AuthenticatedUser(
    Guid Id,
    string Email,
    IReadOnlyCollection<string> Roles);
