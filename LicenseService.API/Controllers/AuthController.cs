using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
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
        // Replace this with real user validation (e.g., from database)
        if (request.Username != "admin" || request.Password != "password123")
            return Unauthorized("Invalid username or password");

        // Hardcoded tenant for testing (later fetched from user/organization table)
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        if (request.Username == "admin" && request.Password == "password123")
        {
            tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var role = "Applicant";
                       
            var user = new
            {
                Id = Guid.NewGuid(),       
                Username = request.Username
            };
            

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, request.Username),
            new Claim("tenant_id", tenantId.ToString()),
            new Claim(ClaimTypes.Role, "Applicant")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            tenantId
        });
    }
        return Unauthorized();
    }

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
}
