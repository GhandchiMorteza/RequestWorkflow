namespace RequestWorkflow.Application.Authentication;

public sealed record RegisterResult(
    AuthenticationResponse? Response,
    IReadOnlyCollection<string> Errors)
{
    public bool Succeeded => Response is not null;

    public static RegisterResult Success(
        AuthenticationResponse response)
    {
        return new RegisterResult(
            response,
            []);
    }

    public static RegisterResult Failure(
        IEnumerable<string> errors)
    {
        return new RegisterResult(
            null,
            [.. errors]);
    }
}
