namespace CozyNestAPIHub.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ReserverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int Status { get; set; }
        public string Notes { get; set; }
    }
}
