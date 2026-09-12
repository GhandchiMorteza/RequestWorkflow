namespace RequestWorkflow.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    AccessToken Generate(AuthenticatedUser user);
}
