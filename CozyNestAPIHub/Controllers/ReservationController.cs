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
                    capacity = item.Capacity,
                    notes = item.Notes
                });
            }
            return Ok(new
            {
                message = "Foglalások sikeresen lekérve.",
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
                    message = "Nincs ilyen szoba."
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
                Notes = request.Notes,
                Capacity = request.Capacity
            };
            if (!await ReservationHandler.IsReservationTimeValid(reservation))
            {
                return BadRequest(new
                {
                    message = "Ennek a foglalásnak az időpontja más foglalással ütközik."
                });
            }
            if (!await ReservationHandler.IsReservationCapacityValid(reservation))
            {
                return BadRequest(new
                {
                    message = "A kapacitás a megengedett értékeken kívülre esett."
                });
            }
            Reservation? createdReservation = await ReservationHandler.CreateReservation(reservation);
            if (createdReservation == null) 
            {
                return StatusCode(500, new
                {
                    message = "Nem sikerült lefoglalni a szobát."
                });
            }
            foreach (var item in request.Services)
            {
                await ReservationHandler.CreateReservationService(new ReservationService() { Quantity = item.Quantity, ReservationId = createdReservation.Id, ServiceId = item.ServiceId });
            }
            return Ok(new
            {
                message = "Sikeres foglalás.",
                reservationData = new { 
                    id = createdReservation.Id,
                    roomNumber = room.RoomNumber,
                    checkInDate = createdReservation.CheckInDate,
                    checkOutDate = createdReservation.CheckOutDate,
                    status = rStatus.Description,
                    notes = createdReservation.Notes,
                    capacity = createdReservation.Capacity
                },
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
                    message = "Nincs ilyen foglalás."
                });
            }
            if (reservation.GuestId != (await GetItemFromContext<User>(HttpContext, "User")).Id)
            {
                return StatusCode(403, new
                {
                    message = "Nem te vagy a foglaló."
                });
            }
            List<ReservationStatus> reservationStatuses = await ReservationHandler.GetReservationStatuses();
            string resDesc = reservationStatuses.First(x => x.Id == reservation.Id).Description;
            if (resDesc == "Cancelled")
            {
                return BadRequest(new
                {
                    message = "A foglalás már lemondottnak számít."
                });
            }
            if (resDesc == "Complete")
            {
                return BadRequest(new
                {
                    message = "Nem lehet befejezett foglalást lemondani."
                });
            }
            reservation.Status = reservationStatuses.First(x => x.Description == "Cancelled").Id;
            Room? room = await RoomHandler.GetRoomById(reservation.RoomId);
            Reservation? updatedReservation = await ReservationHandler.ModifyReservation(reservation);
            if (updatedReservation == null)
            {
                return StatusCode(500, new
                {
                    message = "Nem sikerült a foglalást lemondani."
                });
            }
            return Ok(new
            {
                message = "Foglalás sikeresen lemondva.",
                reservationData = new
                {
                    id = updatedReservation.Id,
                    roomNumber = room.RoomNumber,
                    checkInDate = updatedReservation.CheckInDate,
                    checkOutDate = updatedReservation.CheckOutDate,
                    status = "Cancelled",
                    notes = updatedReservation.Notes,
                    capacity = updatedReservation.Capacity
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
                    status = rStatus.Description,
                    capacity = item.Capacity
                });
            }
            return Ok(new
            {
                message = "Szobák sikeresen lekérve.",
                rooms = final
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
                    status = rStatus.Description,
                    capacity = item.Capacity
                });
            }
            return Ok(new
            {
                message = "Szobák sikeresen lekérve.",
                rooms = finalList
            });
        }
    }
}
