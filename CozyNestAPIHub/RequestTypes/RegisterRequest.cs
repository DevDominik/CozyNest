using CozyNestAPIHub.Interfaces;
using Microsoft.AspNetCore.Identity.Data;

namespace CozyNestAPIHub.RequestTypes
{
    public class RegisterRequest : LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
