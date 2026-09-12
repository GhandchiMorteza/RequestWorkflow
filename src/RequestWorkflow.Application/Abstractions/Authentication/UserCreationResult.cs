namespace RequestWorkflow.Application.Abstractions.Authentication;

public sealed record UserCreationResult(
    AuthenticatedUser? User,
    IReadOnlyCollection<string> Errors)
{
    public bool Succeeded => User is not null;

    public static UserCreationResult Success(
        AuthenticatedUser user)
    {
        return new UserCreationResult(
            user,
            []);
    }

    public static UserCreationResult Failure(
        IEnumerable<string> errors)
    {
        return new UserCreationResult(
            null,
            [.. errors]);
    }
}
