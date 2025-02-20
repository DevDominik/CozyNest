using CozyNestAPIHub.Interfaces;

namespace CozyNestAPIHub.RequestTypes
{
    public class RoomDeleteRequest : IAccessToken
    {
        public string AccessToken { get; set; }
        public int RoomId { get; set; }
    }
}
