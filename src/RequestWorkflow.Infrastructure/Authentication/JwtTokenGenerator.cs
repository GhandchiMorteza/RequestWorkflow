using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RequestWorkflow.Application.Abstractions.Authentication;

namespace RequestWorkflow.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }

    public AccessToken Generate(AuthenticatedUser user)
    {
        var now = DateTimeOffset.UtcNow;

        var expiresAt = now.AddMinutes(
            _options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        claims.AddRange(
            user.Roles.Select(
                role => new Claim("role", role)));

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var tokenValue =
            new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken(
            tokenValue,
            expiresAt);
    }
}
