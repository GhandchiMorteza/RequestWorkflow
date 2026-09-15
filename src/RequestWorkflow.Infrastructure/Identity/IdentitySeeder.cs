using Microsoft.AspNetCore.Identity;
using RequestWorkflow.Application.Abstractions.Authentication;

namespace RequestWorkflow.Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentitySeeder(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(roleManager);
        ArgumentNullException.ThrowIfNull(userManager);

        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync(
        IdentitySeedOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        await SeedRolesAsync();

        await SeedUserAsync(
            "employee@example.com",
            options.EmployeePassword,
            ApplicationRoles.Employee);

        await SeedUserAsync(
            "manager@example.com",
            options.ManagerPassword,
            ApplicationRoles.Manager);

        await SeedUserAsync(
            "finance@example.com",
            options.FinancePassword,
            ApplicationRoles.Finance);
    }

    private async Task SeedRolesAsync()
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
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {FormatErrors(result.Errors)}");
            }
        }
    }

    private async Task SeedUserAsync(
        string email,
        string password,
        string role)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Seed password for '{email}' is missing.");
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email
            };

            var createResult =
                await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create seed user '{email}': " +
                    FormatErrors(createResult.Errors));
            }
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            return;
        }

        var roleResult =
            await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign role '{role}' to '{email}': " +
                FormatErrors(roleResult.Errors));
        }
    }

    private static string FormatErrors(
        IEnumerable<IdentityError> errors)
    {
        return string.Join(
            "; ",
            errors.Select(error => error.Description));
    }
}
