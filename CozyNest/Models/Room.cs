namespace CozyNest.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }

        public int Type { get; set; } // Foreign key to RoomTypes
        public decimal PricePerNight { get; set; }
        public int Status { get; set; } // Foreign key to Statuses
        public string Description { get; set; }
    }
}
