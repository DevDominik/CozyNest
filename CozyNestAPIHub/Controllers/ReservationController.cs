using CozyNestAPIHub.Attributes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
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

            User? user = await UserHandler.GetUserByAccessToken(HttpContext.Items["Token"].ToString());
            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }
            List<Reservation> reservations = await ReservationHandler.GetUserReservations(user);
            List<object> finalList = new List<object>();
            foreach (var item in reservations)
            {
                finalList.Add(new
                {
                    id = item.Id,
                    roomId = item.RoomId,
                    checkInDate = item.CheckInDate,
                    checkOutDate = item.CheckOutDate,
                    status = (await ReservationHandler.GetReservationStatusById(item.Status)).Description,
                    notes = item.Notes
                });
            }
            return Ok(new
            {
                message = "Successfully retrieved reservations.",
                reservations = finalList
            });
        }

    }
}
