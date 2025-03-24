using Microsoft.AspNetCore.Mvc;
using CozyNestAPIHub.RequestTypes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.Attributes;

namespace CozyNestAPIHub.Controllers
{
    /// <summary>
    /// API végpont gyűjtő adminisztrációs funkciókhoz.
    /// </summary>
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
        /// <summary>
        /// Lekéri az összes felhasználót.
        /// </summary>
        /// <returns>Felhasználók listája.</returns>
        /// <response code="200">Sikeres kérés.</response>
        [Route("getusers")]
        [HttpGet]
        [Role("Manager", "Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
            return Ok(new { message = "Sikeres lekérés.", users = usersFinal });
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
        [Route("edituser")]
        [HttpPut]
        [Role("Manager")]
        public async Task<IActionResult> EditUsers([FromBody] UserUpdateRequest request) 
        {
            return Ok();
        }
        [Route("adduser")]
        [HttpPost]
        [Role("Manager")]
        public async Task<IActionResult> AddUser([FromBody] RegisterRequest request)
        {
            return Ok();
        }
        [Route("deleteuser")]
        [HttpDelete]
        [Role("Manager")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            return Ok();
        }
        [Route("addreservation")]
        [HttpPost]
        [Role("Manager", "Receptionist")]
        public async Task<IActionResult> AddReservation([FromBody] ReservationRequest request)
        {
            return Ok();
        }
        [Route("deletereservation")]
        [HttpDelete]
        [Role("Manager", "Receptionist")]
        public async Task<IActionResult> DeleteReservation([FromBody] ReservationCancelRequest request)
        {
            return Ok();
        }
        [Route("modifyreservation")]
        [HttpPut]
        [Role("Manager", "Receptionist")]
        public async Task<IActionResult> ModifyReservation([FromBody] ReservationRequest request)
        {
            return Ok();
        }
        [Route("getreservations")]
        [HttpGet]
        [Role("Manager", "Receptionist")]
        public async Task<IActionResult> GetReservations()
        {
            return Ok();
        }
    }
}
