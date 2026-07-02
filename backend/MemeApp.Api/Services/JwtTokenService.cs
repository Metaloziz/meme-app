using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MemeApp.Api.Services;

public class JwtTokenService(IConfiguration configuration)
{
    public string? GenerateToken(string email)
    {
        var secret = configuration["JWT_SECRET"]
            ?? Environment.GetEnvironmentVariable("JWT_SECRET");

        if (string.IsNullOrWhiteSpace(secret))
        {
            return null;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            claims: [new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Role, "Admin")],
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateAdminCredentials(string email, string password)
    {
        var adminEmail = configuration["ADMIN_EMAIL"]
            ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var adminPassword = configuration["ADMIN_PASSWORD"]
            ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        return !string.IsNullOrWhiteSpace(adminEmail)
            && !string.IsNullOrWhiteSpace(adminPassword)
            && string.Equals(email, adminEmail, StringComparison.OrdinalIgnoreCase)
            && password == adminPassword;
    }
}
