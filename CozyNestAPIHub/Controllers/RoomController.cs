using CozyNestAPIHub.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/room")]
    [ApiController]
    [Role("Manager")]
    public class RoomController : ControllerBase
    {
        [Route("idk")]
        [HttpPost]
        public async Task<IActionResult> GetRooms() 
        {
            return Ok();
        }
    }
}
