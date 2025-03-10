namespace CozyNestAPIHub.RequestTypes
{
    public class UserBulkUpdateRequest
    {
        public List<UserUpdateRequest> UpdatedUsers { get; set; }

    }
}
