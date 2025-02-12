namespace CozyNestAPIHub.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }

        public int Type { get; set; }
        public decimal PricePerNight { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
    }
}
