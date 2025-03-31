using CozyNestAPIHub.Attributes;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;
using CozyNestAPIHub.RequestTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CozyNestAPIHub.Controllers
{
    /// <summary>
    /// API végpont gyűjtő a szolgáltatásokhoz.
    /// </summary>
    [Route("api/service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        /// <summary>
        /// Minden szolgáltatás lekérése lekérdezése.
        /// </summary>
        /// <returns>Szolgáltatások listája.</returns>
        /// <response code="200">Sikeres lekérdezés.</response>
        [Route("services")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                message = "Sikeres szolgáltatás lekérés.",
                services = final
            });
        }
        /// <summary>
        /// Szolgáltatások lekérdezése egy foglaláshoz.
        /// </summary>
        /// <param name="request">Foglalás formázása</param>
        /// <returns>Szolgáltatások listája</returns>
        /// <response code="200">Sikeres lekérdezés.</response>
        /// <response code="401">Nem a foglaló személy.</response>
        /// <response code="404">Foglalás nem található.</response>
        [Route("services")]
        [HttpPost]
        [RequireAccessToken]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Services([FromBody] ReservationServicesRequest request)
        {
            Reservation? reservation = await ReservationHandler.GetReservationById(request.ReservationId);
            if (reservation == null)
            {
                return NotFound(new
                {
                    message = "Foglalás nem található."
                });
            }
            User user = await GetItemFromContext<User>(HttpContext, "User");
            Role role = await GetItemFromContext<Role>(HttpContext, "Role");
            if (reservation.GuestId != user.Id && (role.Name == "Receptionist" || role.Name == "Manager"))
            {
                return Unauthorized(new
                {
                    message = "Nem a te foglalásod."
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
                message = "Sikeres lekérdezés.",
                services = finalServices,
                reservationServices = finalReservationServices
            });
        }
    }
}
