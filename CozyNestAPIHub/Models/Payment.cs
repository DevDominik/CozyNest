namespace CozyNestAPIHub.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }

        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public int PaymentMethod { get; set; }
        public int Status { get; set; }
    }
}
