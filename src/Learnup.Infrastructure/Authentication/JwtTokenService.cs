using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Learnup.Application.Authentication;
using Learnup.Domain.AggregateRoots.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Learnup.Infrastructure.Authentication;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(30);

    public JwtTokenResult CreateToken(User user)
    {
        var jwtOptions = options.Value;
        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JWT secret key is not configured.");
        }

        var expiresAt = DateTime.UtcNow.Add(TokenLifetime);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("mobile_number", user.MobileNumber)
        };

        var token = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
