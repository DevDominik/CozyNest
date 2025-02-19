using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CozyNestAPIHub.RequestTypes;

namespace CozyNestAPIHub.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new 
                { 
                    message = "Invalid username or password." 
                });
            }

            var user = await UserHandler.GetUserByUsername(loginRequest.Username);
            if (user == null || !VerifyPassword(loginRequest.Password, user.HashedPassword))
            {
                return Unauthorized(new 
                { 
                    message = "Invalid credentials." 
                });
            }

            var token = await UserHandler.CreateToken(user);
            if (token == null)
            {
                return StatusCode(500, new 
                { 
                    message = "Token generation failed." 
                });
            }
            Role? role = UserHandler.GetRoleById(user.RoleId);
            return Ok(new
            {
                message = "Login successful.",
                accessToken = token.AccessToken,
                refreshToken = token.RefreshToken,
                userData = new
                {
                    username = user.Username,
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    address = user.Address,
                    closed = user.Closed,
                    email = user.Email,
                    joinDate = user.JoinDate,
                    roleName = role.Name
                }
            });
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null || string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
            {
                return BadRequest(new 
                { 
                    message = "Invalid registration details." 
                });
            }

            if (await UserHandler.UserExists(registerRequest.Username, registerRequest.Email))
            {
                return BadRequest(new 
                { 
                    message = "Username or email already exists." 
                });
            }
            Role? guestRole = UserHandler.GetRoleByName("Guest");
            var user = new User
            {
                Username = registerRequest.Username,
                Email = registerRequest.Email,
                HashedPassword = HashPassword(registerRequest.Password),
                FirstName = "", 
                LastName = "",
                Address = "",
                JoinDate = DateTime.UtcNow,
                RoleId = guestRole.Id
            };
            var createdUser = await UserHandler.CreateUser(user);
            if (createdUser == null)
            {
                return StatusCode(500, new 
                { 
                    message = "User registration failed." 
                });
            }

            return Ok(new 
            { 
                message = "Registration successful.", 
            });
        }

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new 
                { 
                    message = "Tokens are required." 
                });
            }

            bool revoked = await UserHandler.RevokeToken(request.AccessToken, request.RefreshToken);
            if (!revoked)
            {
                return BadRequest(new 
                { 
                    message = "Failed to revoke tokens." 
                });
            }

            return Ok(new 
            { 
                message = "Logged out successfully." 
            });
        }

        [Route("introspect")]
        [HttpPost]
        public async Task<IActionResult> IntrospectToken([FromBody] IntrospectTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.AccessToken))
            {
                return BadRequest(new 
                { 
                    active = false, 
                    message = "Token is required." 
                });
            }

            bool isValid = await UserHandler.ValidateAccessToken(request.AccessToken);
            if (!isValid)
            {
                return Ok(new 
                { 
                    active = false, 
                    message = "Invalid or expired token." 
                });
            }
            User? user = await UserHandler.GetUserByAccessToken(request.AccessToken);
            Role? role = UserHandler.GetRoleById(user.RoleId);
            return Ok(new 
            { 
                active = true, 
                userData = new 
                { 
                    username = user.Username, 
                    id = user.Id, 
                    firstName = user.FirstName, 
                    lastName = user.LastName, 
                    address = user.Address, 
                    closed = user.Closed, 
                    email = user.Email, 
                    joinDate = user.JoinDate, 
                    roleName = role.Name
                } 
            });
        }
        [Route("renewtoken")]
        [HttpPost]
        public async Task<IActionResult> RenewToken([FromBody] RenewTokenRequest request) 
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "Token is required." });
            }
            User? user = await UserHandler.GetUserByRefreshToken(request.RefreshToken);
            if (user == null)
            {
                return BadRequest(new 
                { 
                    message = "Token is invalid." 
                });
            }

            Token? newToken = await UserHandler.CreateToken(user);
            if (newToken == null) 
            {
                return StatusCode(500, new
                {
                    message = "Token generation failed."
                });
            }
            return Ok(new
            {
                message = "Token successfully regenerated.",
                accessToken = newToken.AccessToken,
                refreshToken = newToken.RefreshToken
            });
        }

        [Route("updatedata")]
        [HttpPut]
        public async Task<IActionResult> UpdateData([FromBody] UserSelfUpdateRequest request)
        {
            if (!await UserHandler.ValidateAccessToken(request.AccessToken))
            {
                return Unauthorized(new { message = "Invalid or expired access token." });
            }

            User? user = await UserHandler.GetUserByAccessToken(request.AccessToken);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            bool passwordIsUpdated = false;

            if (!string.IsNullOrWhiteSpace(request.Email)) user.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Username)) user.Username = request.Username;
            if (!string.IsNullOrWhiteSpace(request.Address)) user.Address = request.Address;
            if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.HashedPassword = HashPassword(request.Password);
                passwordIsUpdated = true;
            }

            string? newAccessToken = null;
            string? newRefreshToken = null;

            if (passwordIsUpdated)
            {
                bool success = await UserHandler.RevokeAllTokensForUser(user);
                if ()

                Token? token = await UserHandler.CreateToken(user);
                if (token == null)
                {
                    return StatusCode(500, new
                    {
                        message = "Error arose when creating a new set of tokens."
                    });
                }
                newAccessToken = token.AccessToken;
                newRefreshToken = token.RefreshToken;
            }

            User? updateSuccess = await UserHandler.ModifyUser(user);
            if (updateSuccess == null)
            {
                return StatusCode(500, new { message = "Failed to update user data." });
            }

            string roleName = UserHandler.GetRoleById(user.RoleId)?.Name;

            return Ok(new
            {
                message = "User data updated successfully.",
                userData = new
                {
                    id = user.Id,
                    email = user.Email,
                    username = user.Username,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    closed = user.Closed,
                    joinDate = user.JoinDate,
                    roleName = roleName
                },
                newTokens = passwordIsUpdated ? new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                } : null
            });
        }


        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            string hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }
    }
}
