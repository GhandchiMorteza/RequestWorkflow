using RequestWorkflow.Application.Abstractions.Authentication;
using System.Security.Claims;

namespace RequestWorkflow.Api.Authentication;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var user = GetCurrentUser();

            var userIdValue = user
                .FindFirst("sub")?
                .Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new InvalidOperationException(
                    "Authenticated user identifier is missing.");
            }

            return userId;
        }
    }

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var user = GetCurrentUser();

            return [.. user
                .FindAll("role")
                .Select(claim => claim.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)];
        }
    }

    private ClaimsPrincipal GetCurrentUser()
    {
        var user = _httpContextAccessor
            .HttpContext?
            .User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException(
                "Authenticated user is not available.");
        }

        return user;
    }
}
