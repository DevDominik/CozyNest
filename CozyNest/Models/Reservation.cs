namespace CozyNest.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public string GuestId { get; set; } // Foreign key to ApplicationUser
        public ApplicationUser Guest { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int Status { get; set; } // Foreign key to Statuses
        public string Notes { get; set; }

        public ICollection<ReservationService> ReservationServices { get; set; }
    }
}
