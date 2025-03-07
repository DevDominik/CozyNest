using Microsoft.AspNetCore.Mvc;
using CozyNestAPIHub.RequestTypes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.Attributes;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [RequireAccessToken]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("getusers")]
        [HttpGet]
        [Role("Manager", "Receptionist")]
        public async Task<IActionResult> GetUsers() 
        {
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
                    roleName = (await UserHandler.GetRoleById(loopedUser.RoleId)).Name
                });
            }
            return Ok(new { message = "Request successful.", users = usersFinal });
        }
        [Route("getroles")]
        [HttpGet]
        [Role("Manager", "Receptionist")]
        public async Task<IActionResult> GetRoles()
        {
            List<Role> roleList = await UserHandler.GetRoles();
            List<object> rolesFinal = new List<object>();
            foreach (Role loopedRole in roleList)
            {
                rolesFinal.Add(loopedRole.Name);
            }
            return Ok(new { message = "Request successful.", roles = rolesFinal });
        }
        [Route("editusers")]
        [HttpPut]
        [Role("Manager")]
        public async Task<IActionResult> EditUsers([FromBody] UserBulkUpdateRequest request) 
        {
            return Ok();
        }
    }
}
