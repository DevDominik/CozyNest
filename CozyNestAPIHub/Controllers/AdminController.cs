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
        /// <summary>
        /// Lekéri az összes szerepkört.
        /// </summary>
        /// <returns>Szerepkörök listája</returns>
        /// <response code="200">Sikeres kérés.</response>
        [Route("getroles")]
        [HttpGet]
        [Role("Manager", "Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles()
        {
            List<Role> roleList = await UserHandler.GetRoles();
            List<object> rolesFinal = new List<object>();
            foreach (Role loopedRole in roleList)
            {
                rolesFinal.Add(loopedRole.Name);
            }
            return Ok(new { message = "Sikeresen lekérve az összes szerepkör.", roles = rolesFinal });
        }
        /// <summary>
        /// Módosítja a felhasználó adatait.
        /// </summary>
        /// <param name="request">A módosítási adatok formázása.</param>
        /// <returns>Rendszerüzenet, felhasználói adatok, státuszkód.</returns>
        /// <response code="200">Sikeres módosítás.</response>
        /// <response code="404">Nem található felhasználó vagy szerepkör.</response>
        /// <response code="500">Sikertelen módosítás.</response>
        [Route("modifyuser")]
        [HttpPut]
        [Role("Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModifyUser([FromBody] UserUpdateRequest request) 
        {
            User? user = await UserHandler.GetUserById(request.Id);
            if (user == null)
            {
                return NotFound(new { message = "Felhasználó nem található." });
            }
            Role? role = await UserHandler.GetRoleByName(request.RoleName);
            if (role == null)
            {
                return NotFound(new { message = "Szerepkör nem található." });
            }
            user.Username = request.Username;
            user.Closed = request.Closed;
            user.RoleId = role.Id;
            if (request.PasswordReset)
            {
                user.HashedPassword = HashPassword(GenerateRandomPassword());
            }
            user = await UserHandler.ModifyUser(user);
            if (user == null) 
            { 
                return StatusCode(500, new { message = "Hiba történt a felhasználó módosítása közben." });
            }
            return Ok(new
            {
                message = "Felhasználó módosítva.",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    username = user.Username,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    closed = user.Closed,
                    joinDate = user.JoinDate,
                    roleName = (await UserHandler.GetRoleById(user.RoleId)).Name
                }
            });
        }
        [Route("adduser")]
        [HttpPost]
        [Role("Manager")]
        public async Task<IActionResult> AddUser([FromBody] AdminRegisterRequest request)
        {

            if (await UserHandler.UserExists(request.Username, request.Email))
            {
                return Unauthorized(new
                {
                    message = "Ez a felhasználónév vagy email már foglalt."
                });
            }
            Role? role = await UserHandler.GetRoleByName(request.Role);
            if (role == null)
            {
                return NotFound(new
                {
                    message = "A megadott szerepkör nem található."
                });
            }
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                HashedPassword = HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                JoinDate = DateTime.UtcNow,
                RoleId = role.Id
            };
            var createdUser = await UserHandler.CreateUser(user);
            if (createdUser == null)
            {
                return StatusCode(500, new
                {
                    message = "Sikertelen regisztráció."
                });
            }

            return Ok(new
            {
                message = "Sikeres regisztráció.",
                userData = new {
                    id = createdUser.Id,
                    email = createdUser.Email,
                    username = createdUser.Username,
                    firstName = createdUser.FirstName,
                    lastName = createdUser.LastName,
                    closed = createdUser.Closed,
                    joinDate = createdUser.JoinDate,
                    roleName = role.Name,
                }
            });
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
