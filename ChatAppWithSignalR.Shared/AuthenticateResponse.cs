using ChatAppWithSignalR.Shared.Base;

namespace ChatAppWithSignalR.Shared
{
    public class AuthenticateResponse : BaseResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
