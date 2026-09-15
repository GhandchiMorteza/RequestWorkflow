namespace RequestWorkflow.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid UserId { get; }

    IReadOnlyCollection<string> Roles { get; }
}
