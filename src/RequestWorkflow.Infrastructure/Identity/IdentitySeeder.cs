using Microsoft.AspNetCore.Identity;
using RequestWorkflow.Application.Abstractions.Authentication;

namespace RequestWorkflow.Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public IdentitySeeder(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        ArgumentNullException.ThrowIfNull(roleManager);

        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            ApplicationRoles.Employee,
            ApplicationRoles.Manager,
            ApplicationRoles.Finance
        };

        foreach (var roleName in roles)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await _roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {errors}");
            }
        }
    }
}
