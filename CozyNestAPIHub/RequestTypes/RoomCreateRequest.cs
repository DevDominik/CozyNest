using CozyNestAPIHub.Interfaces;

namespace CozyNestAPIHub.RequestTypes
{
    public class RoomCreateRequest : IAccessToken
    {
        public string AccessToken { get; set; }
        public string RoomNumber { get; set; }

        public string TypeDescription { get; set; }
        public decimal PricePerNight { get; set; }
        public string StatusDescription { get; set; }
        public string Description { get; set; }
    }
}