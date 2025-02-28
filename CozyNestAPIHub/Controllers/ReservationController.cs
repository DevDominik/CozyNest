using CozyNestAPIHub.Attributes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.RequestTypes;
using Microsoft.AspNetCore.Mvc;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/reservation")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        [Route("getreservations")]
        [HttpGet]
        [RequireAccessToken]
        public async Task<IActionResult> GetReservations()
        {
            User user = await GetItemFromContext<User>(HttpContext, "User");
            List<Reservation> reservations = await ReservationHandler.GetUserReservations(user);
            List<object> finalList = new List<object>();
            foreach (var item in reservations)
            {
                Room? room = await RoomHandler.GetRoomById(item.RoomId);
                if (room == null) continue;
                RoomType? rType = await RoomHandler.GetRoomTypeById(room.Type);
                if (rType == null) continue;
                RoomStatus? rStatus = await RoomHandler.GetRoomStatusById(room.Status);
                if (rStatus == null) continue;
                finalList.Add(new
                {
                    id = item.Id,
                    roomId = item.RoomId,
                    roomNumber = room.RoomNumber,
                    roomDescription = room.Description,
                    roomType = rType.Description,
                    checkInDate = item.CheckInDate,
                    checkOutDate = item.CheckOutDate,
                    status = rStatus.Description,
                    notes = item.Notes
                });
            }
            return Ok(new
            {
                message = "Successfully retrieved reservations.",
                reservations = finalList
            });
        }

        [Route("reserve")]
        [HttpPost]
        [RequireAccessToken]
        public async Task<IActionResult> Reserve([FromBody] ReservationRequest request)
        {
            Room? room = await RoomHandler.GetRoomByRoomNumber(request.RoomNumber);
            if (room == null)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }
            User user = await GetItemFromContext<User>(HttpContext, "User");
            ReservationStatus? rStatus = await ReservationHandler.GetReservationStatusByDescription("Incomplete");
            Reservation reservation = new Reservation
            {
                GuestId = user.Id,
                RoomId = room.Id,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                Status = rStatus.Id,
                Notes = request.Notes
            };
            if (!await ReservationHandler.IsReservationValid(reservation))
            {
                return BadRequest(new
                {
                    message = "Reservation times overlap into existing reservations."
                });
            }
            Reservation? createdReservation = await ReservationHandler.CreateReservation(reservation);
            if (createdReservation == null) 
            {
                return StatusCode(500, new
                {
                    message = "Failed to create reservation."
                });
            }
            return Ok(new
            {
                message = "Reservation successfully created.",
                reservationData = new { 
                    id = createdReservation.Id,
                    roomId = createdReservation.RoomId,
                    roomNumber = room.RoomNumber,
                    checkInDate = createdReservation.CheckInDate,
                    checkOutDate = createdReservation.CheckOutDate,
                    status = rStatus.Description,
                    notes = createdReservation.Notes
                }
            });
        }

    }
}
