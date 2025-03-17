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
    [RequireAccessToken]
    [Role("Manager")]
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
                if (roomtype == null || roomstatus == null) { continue; }
                final.Add(new {
                    id = item.Id,
                    roomNumber = item.RoomNumber,
                    type = roomtype.Description,
                    pricePerNight = item.PricePerNight,
                    description = item.Description,
                    deleted = item.Deleted,
                    status = roomstatus.Description,
                    capacity = item.Capacity
                });
            }
            return Ok(new {
                message = "Rooms acquired successfully.",
                rooms = final
            });
        }
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomCreateRequest request)
        {
            if (request == null |
                string.IsNullOrEmpty(request.StatusDescription) ||
                string.IsNullOrEmpty(request.TypeDescription) ||
                string.IsNullOrEmpty(request.RoomNumber) ||
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
                Capacity = request.Capacity
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
                    capacity = createdRoom.Capacity
                }
            });
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] RoomDeleteRequest request) 
        {
            if (request == null)
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
        [Route("modify")]
        [HttpPut]
        public async Task<IActionResult> Modify([FromBody] RoomModifyRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid request." });
            }

            Room? room = await RoomHandler.GetRoomById(request.RoomId);
            if (room == null) { return NotFound(new { message = "Room not found." }); }

            if (!string.IsNullOrWhiteSpace(request.StatusDescription))
            {
                RoomStatus? roomStatus = await RoomHandler.GetRoomStatusByDescription(request.StatusDescription);
                if (roomStatus == null) { return BadRequest(new { message = "Invalid room status." }); }
                room.Status = roomStatus.Id;
            }

            if (!string.IsNullOrWhiteSpace(request.TypeDescription))
            {
                RoomType? roomType = await RoomHandler.GetRoomTypeByDescription(request.TypeDescription);
                if (roomType == null) { return BadRequest(new { message = "Invalid room type." }); }
                room.Type = roomType.Id;
            }

            if (!string.IsNullOrWhiteSpace(request.RoomNumber)) room.RoomNumber = request.RoomNumber;
            if (request.PricePerNight != room.PricePerNight) room.PricePerNight = request.PricePerNight;
            if (!string.IsNullOrWhiteSpace(request.Description)) room.Description = request.Description;

            Room? updateSuccess = await RoomHandler.ModifyRoom(room);
            if (updateSuccess == null) { return StatusCode(500, new { message = "Error arose when updating room." }); }

            string roomStatusDesc = (await RoomHandler.GetRoomStatusById(room.Status))?.Description ?? "Unknown";
            string roomTypeDesc = (await RoomHandler.GetRoomTypeById(room.Type))?.Description ?? "Unknown";

            return Ok(new
            {
                message = "Room data updated successfully.",
                roomData = new
                {
                    id = room.Id,
                    roomNumber = room.RoomNumber,
                    type = roomTypeDesc,
                    pricePerNight = room.PricePerNight,
                    description = room.Description,
                    deleted = room.Deleted,
                    status = roomStatusDesc,
                    capacity = room.Capacity
                }
            });
        }

    }
}
