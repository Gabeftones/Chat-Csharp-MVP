namespace ChatMVC.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserName { get; set; }

        public string RoomName { get; set; }

        public string Text { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}