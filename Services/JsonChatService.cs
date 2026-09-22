using System.Text.Json;
using ChatMVC.Models;

namespace ChatMVC.Services
{
    public class JsonChatService
    {
        private readonly string _baseDirectory;
        private readonly string _usersFilePath;
        private readonly string _roomsFilePath;
        private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public JsonChatService() : this(Path.Combine(Directory.GetCurrentDirectory(), "Data"))
        {
        }

        public JsonChatService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            Directory.CreateDirectory(_baseDirectory);

            _usersFilePath = Path.Combine(_baseDirectory, "UsuariosChat.json");
            _roomsFilePath = Path.Combine(_baseDirectory, "SalasChat.json");

            EnsureSeedData();
        }

        public bool Authenticate(string userName, string password)
        {
            var users = LoadUsers();
            return users.Any(u =>
                string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(u.Password, password, StringComparison.Ordinal));
        }

        public void RegisterUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Usuário e senha são obrigatórios.");
            }

            var users = LoadUsers();
            if (users.Any(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            users.Add(new ChatUser { UserName = userName.Trim(), Password = password.Trim() });
            SaveUsers(users);
        }

        public void CreateRoom(string roomName, IEnumerable<string> participants)
        {
            var sanitizedRoomName = NormalizeRoomName(roomName);
            var members = participants
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (string.IsNullOrWhiteSpace(sanitizedRoomName))
            {
                throw new ArgumentException("Nome da sala é obrigatório.");
            }

            var rooms = LoadRooms();
            var existingRoom = rooms.FirstOrDefault(r => string.Equals(r.RoomName, sanitizedRoomName, StringComparison.OrdinalIgnoreCase));

            if (existingRoom is null)
            {
                rooms.Add(new ChatRoom
                {
                    RoomName = sanitizedRoomName,
                    Participants = members
                });
            }
            else
            {
                foreach (var participant in members)
                {
                    if (!existingRoom.Participants.Any(p => string.Equals(p, participant, StringComparison.OrdinalIgnoreCase)))
                    {
                        existingRoom.Participants.Add(participant);
                    }
                }
            }

            SaveRooms(rooms);
        }

        public List<Message> GetRoomMessages(string roomName)
        {
            var sanitizedRoomName = NormalizeRoomName(roomName);
            var rooms = LoadRooms();
            var room = rooms.FirstOrDefault(r => string.Equals(r.RoomName, sanitizedRoomName, StringComparison.OrdinalIgnoreCase));
            return room?.Messages.OrderBy(m => m.SentAt).ToList() ?? new List<Message>();
        }

        public List<Message> GetRoomHistory(string roomName, string userName)
        {
            var sanitizedRoomName = NormalizeRoomName(roomName);
            var cleanUserName = userName?.Trim();

            if (string.IsNullOrWhiteSpace(cleanUserName))
            {
                throw new UnauthorizedAccessException("Usuário não informado.");
            }

            var rooms = LoadRooms();
            var room = rooms.FirstOrDefault(r => string.Equals(r.RoomName, sanitizedRoomName, StringComparison.OrdinalIgnoreCase));

            if (room is null || !room.Participants.Any(p => string.Equals(p, cleanUserName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedAccessException("Usuário não participa desta sala.");
            }

            return room.Messages.OrderBy(m => m.SentAt).ToList();
        }

        public void SaveMessage(string roomName, string userName, string messageText)
        {
            var sanitizedRoomName = NormalizeRoomName(roomName);
            var cleanUserName = userName?.Trim();
            var cleanMessage = messageText?.Trim();

            if (string.IsNullOrWhiteSpace(cleanUserName) || string.IsNullOrWhiteSpace(cleanMessage))
            {
                return;
            }

            var rooms = LoadRooms();
            var room = rooms.FirstOrDefault(r => string.Equals(r.RoomName, sanitizedRoomName, StringComparison.OrdinalIgnoreCase));

            if (room is null)
            {
                CreateRoom(sanitizedRoomName, new[] { cleanUserName });
                rooms = LoadRooms();
                room = rooms.First(r => string.Equals(r.RoomName, sanitizedRoomName, StringComparison.OrdinalIgnoreCase));
            }

            if (!room.Participants.Any(p => string.Equals(p, cleanUserName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedAccessException("Usuário não tem acesso a esta sala.");
            }

            room.Messages.Add(new Message
            {
                UserName = cleanUserName,
                RoomName = sanitizedRoomName,
                Text = cleanMessage,
                SentAt = DateTime.UtcNow
            });

            SaveRooms(rooms);
        }

        private List<ChatUser> LoadUsers()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<ChatUser>();
            }

            var json = File.ReadAllText(_usersFilePath);
            var users = JsonSerializer.Deserialize<List<ChatUser>>(json);
            return users ?? new List<ChatUser>();
        }

        private void SaveUsers(List<ChatUser> users)
        {
            var json = JsonSerializer.Serialize(users, _jsonOptions);
            File.WriteAllText(_usersFilePath, json);
        }

        private List<ChatRoom> LoadRooms()
        {
            if (!File.Exists(_roomsFilePath))
            {
                return new List<ChatRoom>();
            }

            var json = File.ReadAllText(_roomsFilePath);
            var rooms = JsonSerializer.Deserialize<List<ChatRoom>>(json);
            return rooms ?? new List<ChatRoom>();
        }

        private void SaveRooms(List<ChatRoom> rooms)
        {
            var json = JsonSerializer.Serialize(rooms, _jsonOptions);
            File.WriteAllText(_roomsFilePath, json);
        }

        private static string NormalizeRoomName(string roomName)
        {
            return roomName?.Trim() ?? string.Empty;
        }

        private void EnsureSeedData()
        {
            if (!File.Exists(_usersFilePath))
            {
                var seedUsers = new List<ChatUser>
                {
                    new() { UserName = "admin", Password = "123456" },
                    new() { UserName = "aluno", Password = "123456" },
                    new() { UserName = "professor", Password = "123456" }
                };

                SaveUsers(seedUsers);
            }

            if (!File.Exists(_roomsFilePath))
            {
                SaveRooms(new List<ChatRoom>());
            }
        }
    }
}