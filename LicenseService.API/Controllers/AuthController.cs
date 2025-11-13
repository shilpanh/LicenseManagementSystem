using LicenseService.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LicenseService.API.Domain.Entity;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly LicenseDbContext _dbContext;

    public AuthController(IConfiguration configuration, LicenseDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;     

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Find user from DB
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
            return Unauthorized("Invalid username or password");

        // 2. Verify password
        if (user.PasswordHash != request.Password)
            return Unauthorized("Invalid username or password");
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
                       
            user = new User()
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
