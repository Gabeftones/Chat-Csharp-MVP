namespace ChatMVC.Models
{
    public class ChatViewModel
    {
        public string UserName { get; set; }

        public string RoomName { get; set; }

        public string MessageText { get; set; }

        public List<Message> Messages { get; set; } = new();
    }
}