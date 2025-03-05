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
                    roomNumber = room.RoomNumber,
                    checkInDate = createdReservation.CheckInDate,
                    checkOutDate = createdReservation.CheckOutDate,
                    status = rStatus.Description,
                    notes = createdReservation.Notes
                }
            });
        }

        [Route("cancel")]
        [HttpPost]
        [RequireAccessToken]
        public async Task<IActionResult> Cancel([FromBody] ReservationCancelRequest request) 
        { 
            Reservation? reservation = await ReservationHandler.GetReservationById(request.ReservationId);
            if (reservation == null)
            {
                return NotFound(new
                {
                    message = "Reservation not found."
                });
            }
            if (reservation.GuestId != (await GetItemFromContext<User>(HttpContext, "User")).Id)
            {
                return StatusCode(403, new
                {
                    message = "You are not the reserver."
                });
            }
            List<ReservationStatus> reservationStatuses = await ReservationHandler.GetReservationStatuses();
            string resDesc = reservationStatuses.First(x => x.Id == reservation.Id).Description;
            if (resDesc == "Cancelled")
            {
                return BadRequest(new
                {
                    message = "Reservation already cancelled."
                });
            }
            if (resDesc == "Complete")
            {
                return BadRequest(new
                {
                    message = "Reservation already complete."
                });
            }
            reservation.Status = reservationStatuses.First(x => x.Description == "Cancelled").Id;
            Room? room = await RoomHandler.GetRoomById(reservation.RoomId);
            Reservation? updatedReservation = await ReservationHandler.ModifyReservation(reservation);
            if (updatedReservation == null)
            {
                return StatusCode(500, new
                {
                    message = "Failed to cancel reservation."
                });
            }
            return Ok(new
            {
                message = "Reservation successfully cancelled.",
                reservationData = new
                {
                    id = updatedReservation.Id,
                    roomNumber = room.RoomNumber,
                    checkInDate = updatedReservation.CheckInDate,
                    checkOutDate = updatedReservation.CheckOutDate,
                    status = "Cancelled",
                    notes = updatedReservation.Notes
                }
            });
        }
        [Route("getrooms")]
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            List<Room> roomList = await RoomHandler.GetRooms();
            List<object> final = new List<object>();
            foreach (Room item in roomList)
            {
                RoomStatus? rStatus = await RoomHandler.GetRoomStatusById(item.Status);
                RoomType? rType = await RoomHandler.GetRoomTypeById(item.Type);
                if (rType == null || rStatus == null) { continue; }
                final.Add(new
                {
                    id = item.Id,
                    roomNumber = item.RoomNumber,
                    roomType = rType.Description,
                    pricePerNight = item.PricePerNight,
                    description = item.Description,
                    status = rStatus.Description
                });
            }
            return Ok(new
            {
                message = "Rooms acquired successfully.",
                rooms = final.ToArray()
            });
        }
        [Route("getrooms")]
        [HttpPost]
        public async Task<IActionResult> GetRooms([FromBody] ReservationTimesRequest request)
        {
            List<Room> reservableRooms = await ReservationHandler.GetAvailableRoomsBetweenTimes(request.Start, request.End);
            List<object> finalList = new List<object>();
            foreach (var item in reservableRooms)
            {
                RoomType? rType = await RoomHandler.GetRoomTypeById(item.Type);
                if (rType == null) continue;
                RoomStatus? rStatus = await RoomHandler.GetRoomStatusById(item.Status);
                if (rStatus == null || rStatus.Description != "Available") continue;
                finalList.Add(new
                {
                    id = item.Id,
                    roomNumber = item.RoomNumber,
                    roomType = rType.Description,
                    pricePerNight = item.PricePerNight,
                    description = item.Description,
                    status = rStatus.Description
                });
            }
            return Ok(new
            {
                message = "Successfully retrieved rooms.",
                rooms = finalList
            });
        }
    }
}
