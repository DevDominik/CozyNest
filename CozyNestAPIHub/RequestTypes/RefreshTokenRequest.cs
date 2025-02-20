using CozyNestAPIHub.Interfaces;

namespace CozyNestAPIHub.RequestTypes
{
    public class RefreshTokenRequest : IRefreshToken
    {
        public string RefreshToken { get; set; }
    }
}
