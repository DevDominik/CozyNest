using Microsoft.AspNetCore.Mvc;
using CozyNestAPIHub.RequestTypes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("getusers")]
        [HttpPost]
        public async Task<IActionResult> GetUsers([FromBody] IntrospectTokenRequest request) 
        {

            if (request == null || request.AccessToken == null || request.AccessToken == "") return BadRequest(new { message = "Failed to validate user credentials."});
            if (!await UserHandler.ValidateAccessToken(request.AccessToken)) return Unauthorized(new { message = "Token is invalid or expired." });
            var userId = await UserHandler.GetUserIdByAccessToken(request.AccessToken);
            if (userId == null) return NotFound(new { message = "User was not found." });
            var user = await UserHandler.GetUserById(userId.Value);
            if (UserHandler.GetRoleById(user.RoleId).Name != "Admin") return StatusCode(403, new { message = "Access denied."});
            List<User> userList = await UserHandler.GetUsers();
            List<object> usersFinal = new List<object>();
            foreach (User loopedUser in userList)
            {
                usersFinal.Add(new
                {
                    id = loopedUser.Id,
                    email = loopedUser.Email,
                    username = loopedUser.Username,
                    firstName = loopedUser.FirstName,
                    lastName = loopedUser.LastName,
                    closed = loopedUser.Closed,
                    joinDate = loopedUser.JoinDate,
                    roleName = UserHandler.GetRoleById(loopedUser.RoleId).Name
                });
            }
            return Ok(new { message = "Request successful.", users = usersFinal.ToArray() });
        }
    }
}
