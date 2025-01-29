using CozyNestAPIHub.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CozyNest.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Invalid username or password." });
            }

            if (!_users.TryGetValue(loginRequest.Username, out var user) || user.Password != loginRequest.Password)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null || string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
            {
                return BadRequest(new { message = "Invalid registration details." });
            }

            if (_users.ContainsKey(registerRequest.Username))
            {
                return BadRequest(new { message = "Username already exists." });
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                Password = registerRequest.Password // Store hashed passwords in production
            };

            if (_users.TryAdd(user.UserName, user))
            {
                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(new { message = "Registration failed." });
        }

        [Route("logout")]
        [HttpPost]
        public IActionResult Logout()
        {
            // Stateless JWT authentication means logout is simply handled client-side by discarding the token.
            return Ok(new { message = "Logged out successfully." });
        }

        [Route("introspect")]
        [HttpPost]
        public IActionResult IntrospectToken([FromBody] IntrospectTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { active = false, message = "Token is required." });
            }

            var principal = ValidateJwtToken(request.Token, out var valid);

            if (!valid)
            {
                return Ok(new { active = false, message = "Invalid or expired token." });
            }

            var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var exp = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            return Ok(new
            {
                active = true,
                userId,
                username,
                exp
            });
        }

        private string GenerateJwtToken(User user)
        {
            var authClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(15),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ValidateJwtToken(string token, out bool isValid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No leeway for expiration
                }, out _);

                isValid = true;
                return principal;
            }
            catch
            {
                isValid = false;
                return null;
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class IntrospectTokenRequest
    {
        public string Token { get; set; }
    }
}
