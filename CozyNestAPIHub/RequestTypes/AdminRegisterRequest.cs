using Microsoft.AspNetCore.Identity.Data;

namespace CozyNestAPIHub.RequestTypes
{
    public class AdminRegisterRequest : RegisterRequest
    {
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }
}
