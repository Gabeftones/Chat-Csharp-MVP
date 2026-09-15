using Microsoft.AspNetCore.Mvc;
using ChatMVC.Models;
using ChatMVC.Services;

namespace ChatMVC.Controllers
{
    public class ChatController : Controller
    {
        private readonly JsonChatService _chatService;

        public ChatController(JsonChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Enter(string userName, string
       roomName)
        {
            return RedirectToAction(
            "Room",
            new
            {
                userName,
                roomName
            });
        }

        public IActionResult Room(string userName, string
       roomName)
        {
            ChatViewModel model = new()
            {
                UserName = userName,
                RoomName = roomName,
                Messages =
           _chatService.GetRoomMessages(roomName)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SendMessage(ChatViewModel model)
        {
            Message msg = new()
            {
                UserName = model.UserName,
                RoomName = model.RoomName,
                Text = model.MessageText
            };

            _chatService.SaveMessage(msg);

            return RedirectToAction(
            "Room",
            new
            {
                userName = model.UserName,
                roomName = model.RoomName
            });
        }
    }
}