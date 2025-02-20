namespace CozyNestAPIHub.Models
{
    public class UserUpdateByAdmin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Closed { get; set; }
        public string RoleName { get; set; }
        public bool PasswordReset { get; set; }
    }
}
