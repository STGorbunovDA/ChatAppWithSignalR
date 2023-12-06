namespace ChatAppWithSignalR.Client.Models
{
    public class LastestMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User UserFriendInfo { get; set; }
        public string Content { get; set; }
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
