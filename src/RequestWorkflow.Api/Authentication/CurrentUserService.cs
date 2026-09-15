using RequestWorkflow.Application.Abstractions.Authentication;

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
            var userIdValue = _httpContextAccessor
                .HttpContext?
                .User
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
}