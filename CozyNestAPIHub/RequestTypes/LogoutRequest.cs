namespace CozyNestAPIHub.RequestTypes
{
    public class LogoutRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
