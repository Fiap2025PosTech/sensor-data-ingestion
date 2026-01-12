using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SensorDataIngestion.API.Configuration;
using SensorDataIngestion.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SensorDataIngestion.API.Controllers;

/// <summary>
/// Controller for JWT token generation (development/testing only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthController> logger,
        IWebHostEnvironment environment)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Generates a JWT token for testing (available only in development environment)
    /// </summary>
    /// <param name="request">Token generation data</param>
    /// <returns>JWT Token</returns>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning("Attempt to generate token in production environment");
            return Forbid();
        }

        _logger.LogInformation("Generating JWT token for: {Subject}", request.Subject);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, request.Subject),
            new(ClaimTypes.Name, request.Name ?? request.Subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add custom claims
        if (request.Claims != null)
        {
            foreach (var claim in request.Claims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new TokenResponse
        {
            Token = tokenString,
            ExpiresAt = token.ValidTo,
            TokenType = "Bearer"
        });
    }
}
