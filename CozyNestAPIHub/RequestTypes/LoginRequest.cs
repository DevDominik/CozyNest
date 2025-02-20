using CozyNestAPIHub.Interfaces;

namespace CozyNestAPIHub.RequestTypes
{
    public class LoginRequest : IUsername, IPassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
