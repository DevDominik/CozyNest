using CozyNestAPIHub.Attributes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.RequestTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CozyNestAPIHub.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        [Route("services")]
        [HttpGet]
        public async Task<IActionResult> Services() 
        {
            var services = await ReservationHandler.GetServices();
            var final = new List<object>();
            foreach (var service in services)
            {
                final.Add(new
                {
                    id = service.Id,
                    name = service.Name,
                    description = service.Description,
                    price = service.Price,
                    isActive = service.IsActive
                });
            }
            return Ok(new
            {
                message = "Successfully retrieved services.",
                services = final
            });
        }
        [Route("services")]
        [HttpPost]
        [RequireAccessToken]
        public async Task<IActionResult> Services([FromBody] ReservationServicesRequest request)
        {
            Reservation? reservation = await ReservationHandler.GetReservationById(request.ReservationId);
            if (reservation == null)
            {
                return NotFound(new
                {
                    message = "Reservation not found."
                });
            }
            User user = await GetItemFromContext<User>(HttpContext, "User");
            Role role = await GetItemFromContext<Role>(HttpContext, "Role");
            if (reservation.GuestId != user.Id && role.Name == "Guest")
            {
                return Unauthorized(new
                {
                    message = "Unauthorized."
                });
            }
            var services = await ReservationHandler.GetServices();
            var reservationServices = await ReservationHandler.GetReservationServices(reservation);
            var finalServices = new List<object>();
            var finalReservationServices = new List<object>();
            foreach (var reservationService in reservationServices)
            {
                Service service = services.First(x => x.Id == reservationService.ServiceId);
                finalServices.Add(new
                {
                    id = service.Id,
                    name = service.Name,
                    description = service.Description,
                    price = service.Price,
                    isActive = service.IsActive
                });
                finalReservationServices.Add(new
                {
                    id = reservationService.Id,
                    serviceId = reservationService.ServiceId,
                    quantity = reservationService.Quantity
                });
            }
            return Ok(new
            {
                message = "Successfully retrieved services.",
                services = finalServices,
                reservationServices = finalReservationServices
            });
        }
        [Route("addservice")]
        [HttpPost]
        [RequireAccessToken]
        public async Task<IActionResult> AddService([FromBody] AddServiceToReservationRequest request)
        {
            return Ok();
        }
    }
}
