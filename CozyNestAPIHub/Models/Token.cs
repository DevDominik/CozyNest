namespace CozyNestAPIHub.Models
{
    public class Token
    {
        public long Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessExpiry { get; set; }
        public DateTime RefreshExpiry { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
