namespace CozyNestAPIHub.RequestTypes
{
    public class ReservationRequest
    {
        public string RoomNumber { get; set; }
        public int Capacity { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<AddServiceToReservationRequest> Services { get; set; }
        public string Notes { get; set; }
    }
}
