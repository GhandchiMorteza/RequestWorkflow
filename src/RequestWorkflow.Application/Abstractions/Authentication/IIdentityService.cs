namespace RequestWorkflow.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<UserCreationResult> CreateUserAsync(
        string email,
        string password,
        string role);

    Task<AuthenticatedUser?> ValidateCredentialsAsync(
        string email,
        string password);
}
