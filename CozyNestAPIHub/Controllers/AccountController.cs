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
using CozyNestAPIHub.Attributes;
using Konscious.Security.Cryptography;

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
            Role? role = await UserHandler.GetRoleById(user.RoleId);
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
            Role? guestRole = await UserHandler.GetRoleByName("Guest");
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
        [HttpGet]
        [RequireRefreshToken]
        public async Task<IActionResult> Logout()
        {

            string token = await GetItemFromContext<string>(HttpContext, "Token");

            bool revoked = await UserHandler.RevokeToken(token);
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
        [HttpGet]
        [RequireAccessToken]
        public async Task<IActionResult> IntrospectToken()
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            Role role = await GetItemFromContext<Role>(HttpContext, "Role");
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
        [HttpGet]
        [RequireRefreshToken]
        public async Task<IActionResult> RenewToken() 
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            bool revokeSuccess = await UserHandler.RevokeToken(token);
            if (!revokeSuccess)
            {
                return StatusCode(500, new 
                {
                    message = "Token revoke failure."
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
        [RequireAccessToken]
        public async Task<IActionResult> UpdateData([FromBody] UserSelfUpdateRequest request)
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            
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
                if (!success)
                {
                    return StatusCode(500, new
                    {
                        message = "Failed to revoke all tokens for user."
                    });
                }
                Token? newToken = await UserHandler.CreateToken(user);
                if (newToken == null)
                {
                    return StatusCode(500, new
                    {
                        message = "Error arose when creating a new set of tokens."
                    });
                }
                newAccessToken = newToken.AccessToken;
                newRefreshToken = newToken.RefreshToken;
            }

            User? updateSuccess = await UserHandler.ModifyUser(user);
            if (updateSuccess == null)
            {
                return StatusCode(500, new { message = "Failed to update user data." });
            }

            Role role = await GetItemFromContext<Role>(HttpContext, "Role");

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
                    roleName = role.Name
                },
                newTokens = passwordIsUpdated ? new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                } : null
            });
        }

        [Route("deleteaccount")]
        [HttpDelete]
        [RequireAccessToken]
        public async Task<IActionResult> DeleteAccount()
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            if (user.Closed)
            {
                return Unauthorized(new { message = "Account already closed."});
            }
            user.Closed = true;
            User? updatedUser = await UserHandler.ModifyUser(user);
            if (updatedUser == null)
            {
                return StatusCode(500, new { message = "Failed to close account." });
            }
            bool success = await UserHandler.RevokeAllTokensForUser(user);
            if (!success)
            {
                return StatusCode(500, new { message = "Failed to revoke all tokens for user." });
            }
            return Ok(new { message = "Account closed successfully." });
        }

        [Route("logouteverywhere")]
        [HttpGet]
        [RequireAccessToken]
        public async Task<IActionResult> LogoutEverywhere()
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            bool success = await UserHandler.RevokeAllTokensForUser(user);
            if (!success)
            {
                return StatusCode(500, new { message = "Failed to revoke all tokens for user." });
            }
            return Ok(new { message = "Logged out from all devices." });
        }

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 4;
                argon2.MemorySize = 65536;
                argon2.Iterations = 3;

                byte[] hash = argon2.GetBytes(32);
                return Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('$');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 4;
                argon2.MemorySize = 65536;
                argon2.Iterations = 3;

                byte[] computedHash = argon2.GetBytes(32);
                return storedHash.SequenceEqual(computedHash);
            }
        }
    }
}
