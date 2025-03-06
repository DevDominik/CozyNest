namespace CozyNestAPIHub.RequestTypes
{
    public class AddServiceToReservationRequest
    {
        public int ReservationId { get; set; }
        public int ServiceId { get; set; }
    }
}
