using CozyNestAPIHub.Interfaces;

namespace CozyNestAPIHub.RequestTypes
{
    public class AccessTokenRequest : IAccessToken
    {
        public string AccessToken { get; set; }
    }
}
