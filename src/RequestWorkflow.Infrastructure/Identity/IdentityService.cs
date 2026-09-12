using Microsoft.AspNetCore.Identity;
using RequestWorkflow.Application.Abstractions.Authentication;

namespace RequestWorkflow.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(userManager);

        _userManager = userManager;
    }

    public async Task<UserCreationResult> CreateUserAsync(
        string email,
        string password,
        string role)
    {
        var normalizedEmail = email.Trim();

        var existingUser =
            await _userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            return UserCreationResult.Failure(
                ["Email is already registered."]);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            UserName = normalizedEmail
        };

        var createResult =
            await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return UserCreationResult.Failure(
                createResult.Errors.Select(error => error.Description));
        }

        var roleResult =
            await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return UserCreationResult.Failure(
                roleResult.Errors.Select(error => error.Description));
        }

        var authenticatedUser = new AuthenticatedUser(
            user.Id,
            user.Email!,
            [role]);

        return UserCreationResult.Success(authenticatedUser);
    }

    public async Task<AuthenticatedUser?> ValidateCredentialsAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user is null)
        {
            return null;
        }

        var passwordIsValid =
            await _userManager.CheckPasswordAsync(user, password);

        if (!passwordIsValid)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthenticatedUser(
            user.Id,
            user.Email!,
            roles.ToArray());
    }
}
