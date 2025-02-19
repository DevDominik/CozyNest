namespace CozyNestAPIHub.RequestTypes
{
    public class RoomModifyRequest : RoomCreateRequest
    {
        public int RoomId { get; set; }
        public bool Deleted { get; set; }
    }
}
