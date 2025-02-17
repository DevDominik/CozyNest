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
    public class RoomController : ControllerBase
    {
        [Route("list")]
        [HttpGet]
        public async Task<IActionResult> List() 
        {
            List<Room> roomList = await RoomHandler.GetRooms();
            List<object> final = new List<object>();
            foreach (Room item in roomList)
            {
                RoomStatus? roomstatus = await RoomHandler.GetRoomStatusById(item.Status);
                RoomType? roomtype = await RoomHandler.GetRoomTypeById(item.Type);
                final.Add(new {
                    id = item.Id,
                    roomNumber = item.RoomNumber,
                    type = roomtype.Description,
                    pricePerNight = item.PricePerNight,
                    description = item.Description,
                    deleted = item.Deleted,
                    status = roomstatus.Description
                });
            }
            return Ok(new {
                message = "Rooms acquired successfully.",
                rooms = final.ToArray()
            });
        }

        
    }
}
