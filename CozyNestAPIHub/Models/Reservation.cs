namespace CozyNestAPIHub.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public User Guest { get; set; }
        public Room Room { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int Status { get; set; }
        public string Notes { get; set; }

        public List<ReservationService> ReservationServices { get; set; }
    }
}
