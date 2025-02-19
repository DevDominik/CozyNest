using Microsoft.AspNetCore.Mvc;
using CozyNestAPIHub.RequestTypes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.Attributes;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Role("Manager", "Receptionist")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("getusers")]
        [HttpPost]
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
                    roleName = UserHandler.GetRoleById(loopedUser.RoleId).Name
                });
            }
            return Ok(new { message = "Request successful.", users = usersFinal.ToArray() });
        }
    }
}
