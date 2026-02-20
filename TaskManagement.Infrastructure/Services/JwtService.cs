using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(AppUser? user)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Claims are pieces of information about the user
        // that get embedded inside the token payload.
        // Anyone with the token can read these (they're base64 encoded, not encrypted)
        // so never put sensitive data like passwords in claims.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            // Jti is a unique ID for this specific token
            // Useful for token revocation if needed later
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // The signing key — this is the secret that proves
        // the token came from YOUR server and hasn't been tampered with.
        // If someone gets this key, they can forge tokens for any user.
        // This is why it lives in appsettings and never in code.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            // Token expires in 24 hours
            // In production, access tokens are shorter (15-60 min)
            // and you use refresh tokens for longer sessions
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
