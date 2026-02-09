using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var adminEmail = _configuration["AdminCredentials:Email"];
        var adminPassword = _configuration["AdminCredentials:Password"];

        if (request.Email != adminEmail || request.Password != adminPassword)
        {
            return Unauthorized(new { Error = "არასწორი ელ-ფოსტა ან პაროლი" });
        }

        var token = GenerateJwtToken(request.Email);
        var expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!));

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiration.ToString("o")
        });
    }

    private string GenerateJwtToken(string email)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string? ExpiresAt { get; set; }
}
