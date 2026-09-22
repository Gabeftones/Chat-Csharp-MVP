namespace ChatMVC.Models
{
    public class ChatRoom
    {
        public string RoomName { get; set; } = string.Empty;

        public List<string> Participants { get; set; } = new();

        public List<Message> Messages { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
