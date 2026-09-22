using ChatMVC.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatMVC.Hubs;

public class ChatHub : Hub
{
    private readonly JsonChatService _chatService;

    public ChatHub(JsonChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinRoom(string roomName, string userName)
    {
        if (string.IsNullOrWhiteSpace(roomName) || string.IsNullOrWhiteSpace(userName))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("ReceiveSystemMessage", $"{userName} entrou na sala {roomName}.");
    }

    public async Task SendMessage(string roomName, string userName, string message)
    {
        if (string.IsNullOrWhiteSpace(roomName) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var text = message.Trim();
        _chatService.SaveMessage(roomName, userName, text);

        await Clients.Group(roomName).SendAsync("ReceiveMessage", new
        {
            userName,
            roomName,
            text,
            sentAt = DateTime.UtcNow
        });
    }
}
