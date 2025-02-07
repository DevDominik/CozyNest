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

            var token = await UserHandler.CreateToken(user.Id, user.Username);
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
                FirstName = "",  // Default values, can be updated later
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
            Token? generatedToken = await UserHandler.CreateToken(createdUser.Id, createdUser.Username);

            return Ok(new 
            { 
                message = "Registration successful.", 
                accessToken = generatedToken.AccessToken, 
                refreshToken = generatedToken.RefreshToken,
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
                    roleName = guestRole.Name 
                }
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
            int? userid = await UserHandler.GetUserIdByAccessToken(request.AccessToken);
            User? user = await UserHandler.GetUserById(userid.Value);
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
            int? userId = await UserHandler.GetUserIdByRefreshToken(request.RefreshToken);
            bool isRefreshTokenValidated = await UserHandler.ValidateRefreshToken(request.RefreshToken);
            if (userId == null || !isRefreshTokenValidated)
            {
                return BadRequest(new 
                { 
                    message = "Token is invalid." 
                });
            }

            if (userId.HasValue)
            {
                User? user = await UserHandler.GetUserById(userId.Value);
                Token? newToken = await UserHandler.CreateToken(user.Id, user.Username);
                return Ok(new 
                { 
                    message = "Token successfully regenerated.", 
                    accessToken = newToken.AccessToken, 
                    refreshToken = newToken.RefreshToken
                });
            }
            return StatusCode(500, new { message = "Unknown error." });
        }
        [Route("updatedata")]
        [HttpPost]
        public async Task<IActionResult> UpdateData([FromBody] UserSelfUpdateRequest request)
        {
            // Validate the access token
            bool isValid = await UserHandler.ValidateAccessToken(request.AccessToken);
            if (!isValid) { return Unauthorized(new { message = "Invalid or expired access token." }); }

            // Retrieve the user ID associated with the access token
            int? userId = await UserHandler.GetUserIdByAccessToken(request.AccessToken);
            if (userId == null || !userId.HasValue) { return NotFound(new { message = "User not found." }); }

            // Fetch the user entity from the database
            User? user = await UserHandler.GetUserById(userId.Value);
            if (user == null) { return NotFound(new { message = "User data not found." }); }

            // Update user properties based on the request (assuming some fields are updatable)
            if (!string.IsNullOrWhiteSpace(request.Email)) user.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Username)) user.Username = request.Username;
            if (!string.IsNullOrWhiteSpace(request.Address)) user.Address = request.Address;
            if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.Password)) user.HashedPassword = HashPassword(request.Password);

            // Save changes to the database
            User? updateSuccess = await UserHandler.ModifyUser(user);
            if (updateSuccess == null) { return StatusCode(500, new { message = "Failed to update user data." }); }

            return Ok(new { message = "User data updated successfully." });
        }


        // Hash the password before storing it
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Verify the password against the stored hash
        private static bool VerifyPassword(string password, string storedHash)
        {
            string hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }
    }
}
