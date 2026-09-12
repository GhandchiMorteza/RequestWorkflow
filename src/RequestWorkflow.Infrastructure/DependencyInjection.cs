using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Abstractions.Persistence;
using RequestWorkflow.Infrastructure.Authentication;
using RequestWorkflow.Infrastructure.Identity;
using RequestWorkflow.Infrastructure.Persistence;
using RequestWorkflow.Infrastructure.Persistence.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace RequestWorkflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{JwtOptions.SectionName}' is missing.");

        ValidateJwtOptions(jwtOptions);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped(
            typeof(IGenericRepository<>),
            typeof(GenericRepository<>));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddSingleton(jwtOptions);

        services.AddSingleton<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtOptions.SecretKey)),

                        ValidateLifetime = true,

                        NameClaimType =
                            JwtRegisteredClaimNames.Email,

                        RoleClaimType = "role",

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

        services.AddScoped<IdentitySeeder>();

        return services;
    }

    private static void ValidateJwtOptions(
        JwtOptions jwtOptions)
    {
        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
        {
            throw new InvalidOperationException(
                "Jwt:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new InvalidOperationException(
                "Jwt:Audience is required.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException(
                "Jwt:SecretKey is required.");
        }

        if (Encoding.UTF8.GetByteCount(jwtOptions.SecretKey) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:SecretKey must be at least 32 bytes.");
        }

        if (jwtOptions.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "Jwt:ExpirationMinutes must be greater than zero.");
        }
    }
}
