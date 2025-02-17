using CozyNestAPIHub.Attributes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.RequestTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/room")]
    [ApiController]
    [Role("Receptionist", "Manager")]
    public class RoomController : ControllerBase
    {
        [Route("list")]
        [HttpPost]
        public async Task<IActionResult> List() 
        {
            List<Room> roomList = await RoomHandler.GetRooms();
            List<object> final = new List<object>();
            foreach (Room item in roomList)
            {
                RoomStatus? roomstatus = await RoomHandler.GetRoomStatusById(item.Status);
                final.Add(new {
                    id = item.Id,
                    roomNumber = item.RoomNumber,
                    type = roomstatus.Description,
                    pricePerNight = item.PricePerNight,
                    description = item.Description,
                    deleted = item.Deleted
                });
            }
            return Ok(new {
                message = "Rooms acquired successfully.",
                rooms = final.ToArray()
            });
        }

        
    }
}
