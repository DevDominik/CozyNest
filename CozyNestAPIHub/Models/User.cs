using Microsoft.AspNetCore.Identity;

namespace CozyNestAPIHub.Models
{
    public class User
    {
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public bool Closed { get; set; }
    }
}
