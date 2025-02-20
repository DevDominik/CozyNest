using CozyNestAPIHub.Models;

namespace CozyNestAPIHub.RequestTypes
{
    public class UserBulkUpdateRequest
    {
        public List<UserUpdateByAdmin> UpdatedUsers { get; set; }

    }
}
