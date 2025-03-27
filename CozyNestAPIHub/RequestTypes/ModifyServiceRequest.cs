namespace CozyNestAPIHub.RequestTypes
{
    public class ModifyServiceRequest : AddServiceRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
