namespace CozyNestAPIHub.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int Capacity { get; set; }
    }
}
