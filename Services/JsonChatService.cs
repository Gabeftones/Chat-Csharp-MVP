using System.Text.Json;
using ChatMVC.Models;

namespace ChatMVC.Services
{
    public class JsonChatService
    {
        private readonly string _filePath;

        public JsonChatService()
        {
            _filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Data",
            "messages.json");

            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory("Data");
                File.WriteAllText(_filePath, "[]");
            }
        }

        public List<Message> LoadMessages()
        {
            var json = File.ReadAllText(_filePath);

            var messages =
           JsonSerializer.Deserialize<List<Message>>(json);

            return messages ?? new List<Message>();
        }

        public void SaveMessage(Message message)
        {
            var messages = LoadMessages();
            messages.Add(message);

            var json = JsonSerializer.Serialize(
            messages,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }

        public List<Message> GetRoomMessages(string room)
        {
            return LoadMessages()
            .Where(x => x.RoomName == room)
            .OrderBy(x => x.SentAt)
            .ToList();
        }
    }
}