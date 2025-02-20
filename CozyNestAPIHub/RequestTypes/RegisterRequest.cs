using Microsoft.AspNetCore.Identity.Data;

namespace CozyNestAPIHub.RequestTypes
{
    public class RegisterRequest : LoginRequest
    {
        public string Email { get; set; }
    }
}
