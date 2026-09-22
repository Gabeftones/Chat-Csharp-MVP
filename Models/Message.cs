namespace ChatMVC.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserName { get; set; } = string.Empty;

        public string RoomName { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}