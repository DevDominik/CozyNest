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
    /// <summary>
    /// API végpont gyűjtő a felhasználói műveletekhez.
    /// </summary>
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Bejelentkezési végpont.
        /// </summary>
        /// <param name="loginRequest">A megszabott kérés formázása.</param>
        /// <returns>Felhasználási adatokat, tokeneket.</returns>
        /// <response code="200">Sikeres bejelentkezés.</response>
        /// <response code="400">Nem megfelelő formázás.</response>
        /// <response code="401">Hibás felhasználónév vagy jelszó.</response>
        /// <response code="403">Felhasználói fiók zárolva.</response>
        /// <response code="500">Token generációs hiba.</response>
        [Route("login")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new 
                { 
                    message = "Üres adatot a rendszer nem enged." 
                });
            }

            var user = await UserHandler.GetUserByUsername(loginRequest.Username);
            if (user == null || !VerifyPassword(loginRequest.Password, user.HashedPassword))
            {
                return Unauthorized(new 
                { 
                    message = "Érvénytelen felhasználónév vagy jelszó." 
                });
            }
            if (user.Closed)
            {
                return new ObjectResult(new
                {
                    message = "Felhasználói fiók zárolva."
                })
                { StatusCode = 403 };
            }
            var token = await UserHandler.CreateToken(user);
            if (token == null)
            {
                return StatusCode(500, new 
                { 
                    message = "Szerverhiba lépett fel." 
                });
            }
            Role? role = await UserHandler.GetRoleById(user.RoleId);
            return Ok(new
            {
                message = "Sikeres bejelentkezés.",
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
        /// <summary>
        /// Regisztrációs végpont.
        /// </summary>
        /// <param name="registerRequest">Regisztrálási adatok formázása.</param>
        /// <returns>Rendszerüzenet státuszkóddal.</returns>
        /// <response code="200">Sikeres regisztráció.</response>
        /// <response code="400">Nem megfelelő formázás.</response>
        /// <response code="401">Már van ilyen felhasználó.</response>
        /// <response code="500">Sikertelen regisztráció.</response>
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null || string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
            {
                return BadRequest(new 
                { 
                    message = "Érvénytelen regisztrációs adatok." 
                });
            }

            if (await UserHandler.UserExists(registerRequest.Username, registerRequest.Email))
            {
                return Unauthorized(new 
                { 
                    message = "Ez a felhasználónév vagy email már foglalt." 
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
                    message = "Sikertelen regisztráció." 
                });
            }

            return Ok(new 
            { 
                message = "Sikeres regisztráció.", 
            });
        }
        /// <summary>
        /// Kijelentkezési végpont.
        /// </summary>
        /// <returns>Rendszerüzenet státuszkóddal.</returns>
        /// <response code="200">Sikeres kijelentkezés.</response>
        /// <response code="401">Sikertelen kijelentkezés.</response>
        [Route("logout")]
        [HttpGet]
        [RequireRefreshToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            return await UserHandler.RevokeToken(await GetItemFromContext<string>(HttpContext, "Token")) 
                ? Ok(new 
            { 
                message = "Sikeres kijelentkezés." 
            }) 
                : Unauthorized(new
            {
                message = "Nem sikerült kijelentkezni."
            }); ;
        }
        /// <summary>
        /// Bejelentkezettség hitelesítése.
        /// </summary>
        /// <returns>Felhasználói adatok, szerepkör.</returns>
        /// <response code="200">Sikeres adatlekérés.</response>
        [Route("introspect")]
        [HttpGet]
        [RequireAccessToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> IntrospectToken()
        {
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
        /// <summary>
        /// Új tokenpáros kérése.
        /// </summary>
        /// <returns>Új AccessToken és RefreshToken.</returns>
        /// <response code="200">Sikeres tokenlekérdezés.</response>
        /// <response code="500">Tokenműveletek elbuktak.</response>
        [Route("renewtoken")]
        [HttpGet]
        [RequireRefreshToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RenewToken() 
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            bool revokeSuccess = await UserHandler.RevokeToken(token);
            if (!revokeSuccess)
            {
                return StatusCode(500, new 
                {
                    message = "Szerverhiba lépett fel."
                });
            }

            Token? newToken = await UserHandler.CreateToken(user);
            if (newToken == null) 
            {
                return StatusCode(500, new
                {
                    message = "Szerverhiba lépett fel."
                });
            }
            return Ok(new
            {
                message = "Sikeres tokenpáros kérés.",
                accessToken = newToken.AccessToken,
                refreshToken = newToken.RefreshToken
            });
        }
        /// <summary>
        /// Felhasználói adatok frissítése.
        /// </summary>
        /// <param name="request">A frissítési kérés formázása.</param>
        /// <returns>Felhasználói adatok és token páros ha a jelszó frissítve van.</returns>
        /// <response code="200">Sikeres adatfrissítés.</response>
        /// <response code="500">Adatfrissítés elbukott szerverhiba miatt.</response>
        [Route("updatedata")]
        [HttpPut]
        [RequireAccessToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                if (!await UserHandler.RevokeAllTokensForUser(user))
                {
                    return StatusCode(500, new
                    {
                        message = "Szerverhiba lépett fel."
                    });
                }
                Token? newToken = await UserHandler.CreateToken(user);
                if (newToken == null)
                {
                    return StatusCode(500, new
                    {
                        message = "Szerverhiba lépett fel."
                    });
                }
                newAccessToken = newToken.AccessToken;
                newRefreshToken = newToken.RefreshToken;
            }

            User? updateSuccess = await UserHandler.ModifyUser(user);
            if (updateSuccess == null)
            {
                return StatusCode(500, new { message = "Nem sikerült frissíteni a felhasználói adatokat." });
            }

            Role role = await GetItemFromContext<Role>(HttpContext, "Role");

            return Ok(new
            {
                message = "Felhasználói adatok sikeresen frissítve.",
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
        /// <summary>
        /// Lezárja a felhasználó fiókját.
        /// </summary>
        /// <returns>Rendszerüzenet státuszkóddal.</returns>
        /// <response code="200">Sikeres fióklezárás.</response>
        /// <response code="401">A fiók már lezárva.</response>
        /// <response code="500">Sikertelen fióklezárás.</response>
        [Route("deleteaccount")]
        [HttpDelete]
        [RequireAccessToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAccount()
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            if (user.Closed)
            {
                return Unauthorized(new { message = "A fiók már le van zárva."});
            }
            user.Closed = true;
            User? updatedUser = await UserHandler.ModifyUser(user);
            if (updatedUser == null)
            {
                return StatusCode(500, new { message = "Nem sikerült lezárni a fiókot." });
            }
            bool success = await UserHandler.RevokeAllTokensForUser(user);
            if (!success)
            {
                return StatusCode(500, new { message = "Szerverhiba lépett fel." });
            }
            return Ok(new { message = "Sikeresen lezárva a fiók." });
        }
        /// <summary>
        /// Kijelentkezés minden eszközről.
        /// </summary>
        /// <returns>Rendszerüzenet státuszkóddal.</returns>
        /// <response code="200">Sikeres kijelentkezés minden eszközről.</response>
        /// <response code="500">Token érvénytelenítési hiba lépett fel.</response>
        [Route("logouteverywhere")]
        [HttpGet]
        [RequireAccessToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogoutEverywhere()
        {
            string token = await GetItemFromContext<string>(HttpContext, "Token");
            User user = await GetItemFromContext<User>(HttpContext, "User");
            bool success = await UserHandler.RevokeAllTokensForUser(user);
            if (!success)
            {
                return StatusCode(500, new { message = "Szerverhiba lépett fel." });
            }
            return Ok(new { message = "Minden eszközről kijelentkezve sikeresen." });
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
