using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CloudHosting.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);
            
            // Simple validation
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Login failed: Empty username or password");
                return BadRequest(new { Error = "Username and password are required" });
            }

            // demo validation
            if (request.Username == "demo" && request.Password == "password")
            {
                var token = GenerateJwtToken(request.Username);
                _logger.LogInformation("Login successful for user: {Username}", request.Username);
                return Ok(new { Token = token });
            }

            _logger.LogWarning("Login failed for user: {Username}", request.Username);
            return Unauthorized(new { Error = "Invalid credentials" });
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("Authenticated request from user: {Username}", username);
            return Ok(new { Message = $"Hello, {username}! You are authenticated." });
        }

        private string GenerateJwtToken(string username)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured"));
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, "1") // change
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"] ?? "CloudHosting",
                Audience = _configuration["Jwt:Audience"] ?? "CloudHostingUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}