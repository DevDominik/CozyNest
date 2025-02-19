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
        [Route("create")]
        [HttpPost]
        [Role("Manager")]
        public async Task<IActionResult> Create([FromBody] RoomCreateRequest request)
        {
            if (request == null |
                string.IsNullOrEmpty(request.StatusDescription) |
                string.IsNullOrEmpty(request.TypeDescription) |
                string.IsNullOrEmpty(request.RoomNumber) |
                request.PricePerNight == null |
                request.Description == null)
            {
                return BadRequest(new
                {
                    message = "Invalid request."
                });
            }
            RoomStatus? roomStatus = await RoomHandler.GetRoomStatusByDescription(request.StatusDescription);
            if (roomStatus == null)
            {
                return BadRequest(new
                {
                    message = "Invalid room status."
                });
            }
            RoomType? roomType = await RoomHandler.GetRoomTypeByDescription(request.TypeDescription);
            if (roomType == null)
            {
                return BadRequest(new
                {
                    message = "Invalid room type."
                });
            }
            Room room = new Room()
            {
                Deleted = false,
                Description = request.Description,
                RoomNumber = request.RoomNumber,
                Status = roomStatus.Id,
                Type = roomType.Id,
                PricePerNight = request.PricePerNight,
            };

            Room? createdRoom = await RoomHandler.CreateRoom(room);
            if (createdRoom == null) 
            {
                return StatusCode(500, new
                {
                    message = "Error arose when creating room."
                });
            }
            return Ok(new
            {
                message = "New room created.",
                roomData = new {
                    id = createdRoom.Id,
                    description = createdRoom.Description,
                    status = roomStatus.Description,
                    type = roomType.Description,
                    pricePerNight = createdRoom.PricePerNight,
                    deleted = createdRoom.Deleted,
                    roomNumber = createdRoom.RoomNumber,
                }
            });
        }

        [Route("delete")]
        [HttpDelete]
        [Role("Manager")]
        public async Task<IActionResult> Delete([FromBody] RoomDeleteRequest request) 
        {
            if (request == null || request.RoomId == null)
            {
                return BadRequest(new 
                { 
                    message = "Invalid request."    
                });
            }
            Room? room = await RoomHandler.GetRoomById(request.RoomId);
            if (room == null) 
            { 
                return NotFound(new 
                {
                    message = "Room not found."
                });
            }
            if (room.Deleted)
            {
                return Unauthorized(new
                {
                    message = "Room is already deleted."
                });
            }
            room.Deleted = true;
            if (await RoomHandler.ModifyRoom(room) == null) 
            {
                return StatusCode(500, new 
                {
                    message = "Error arose when deleting room."
                });
            }
            return Ok(new
            {
                message = "Successfully deleted room.",
                roomId = room.Id,
            });
        }
    }
}
