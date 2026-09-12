using RequestWorkflow.Application.Abstractions.Authentication;

namespace RequestWorkflow.Application.Authentication;

public sealed class AuthenticationService
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        ArgumentNullException.ThrowIfNull(identityService);
        ArgumentNullException.ThrowIfNull(jwtTokenGenerator);

        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RegisterResult> RegisterAsync(
        string email,
        string password)
    {
        var result = await _identityService.CreateUserAsync(
            email,
            password,
            ApplicationRoles.Employee);

        if (!result.Succeeded)
        {
            return RegisterResult.Failure(result.Errors);
        }

        var response = CreateAuthenticationResponse(result.User!);

        return RegisterResult.Success(response);
    }

    public async Task<AuthenticationResponse?> LoginAsync(
        string email,
        string password)
    {
        var user = await _identityService.ValidateCredentialsAsync(
            email,
            password);

        if (user is null)
        {
            return null;
        }

        return CreateAuthenticationResponse(user);
    }

    private AuthenticationResponse CreateAuthenticationResponse(
        AuthenticatedUser user)
    {
        var token = _jwtTokenGenerator.Generate(user);

        return new AuthenticationResponse(
            user.Id,
            user.Email,
            user.Roles,
            token.Value,
            token.ExpiresAt);
    }
}
