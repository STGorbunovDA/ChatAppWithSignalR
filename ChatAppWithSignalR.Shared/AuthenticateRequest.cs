namespace ChatAppWithSignalR.Shared
{
    public class AuthenticateRequest
    {
        public string LoginId { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
