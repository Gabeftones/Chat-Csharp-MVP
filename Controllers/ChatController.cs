using ChatMVC.Models;
using ChatMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatMVC.Controllers
{
    public class ChatController : Controller
    {
        private readonly JsonChatService _chatService;

        public ChatController(JsonChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult Room(string roomName)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return RedirectToAction("Index", "Home");
            }

            var normalizedRoom = string.IsNullOrWhiteSpace(roomName) ? "sala-geral" : roomName.Trim();
            _chatService.CreateRoom(normalizedRoom, new[] { currentUser });

            var model = new ChatViewModel
            {
                UserName = currentUser,
                RoomName = normalizedRoom,
                Messages = _chatService.GetRoomHistory(normalizedRoom, currentUser)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SendMessage(ChatViewModel model)
        {
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return RedirectToAction("Index", "Home");
            }

            var userName = string.IsNullOrWhiteSpace(model.UserName) ? currentUser : model.UserName;
            if (string.IsNullOrWhiteSpace(model.RoomName))
            {
                model.RoomName = "sala-geral";
            }

            _chatService.SaveMessage(model.RoomName, userName, model.MessageText);

            return RedirectToAction("Room", new { roomName = model.RoomName });
        }
    }
}