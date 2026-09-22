using System.Diagnostics;
using ChatMVC.Models;
using ChatMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatMVC.Controllers;

public class HomeController : Controller
{
    private readonly JsonChatService _chatService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(JsonChatService chatService, ILogger<HomeController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("CurrentUser")))
        {
            return RedirectToAction("Room", "Chat", new { roomName = "sala-geral" });
        }

        return View();
    }

    [HttpPost]
    public IActionResult Login(string userName, string password, string roomName)
    {
        var cleanUserName = userName?.Trim();
        var cleanPassword = password?.Trim();
        var cleanRoomName = string.IsNullOrWhiteSpace(roomName) ? "sala-geral" : roomName.Trim();

        if (string.IsNullOrWhiteSpace(cleanUserName) || string.IsNullOrWhiteSpace(cleanPassword))
        {
            ViewBag.Error = "Informe usuário e senha.";
            return View("Index");
        }

        if (!_chatService.Authenticate(cleanUserName, cleanPassword))
        {
            ViewBag.Error = "Usuário ou senha inválidos.";
            return View("Index");
        }

        HttpContext.Session.SetString("CurrentUser", cleanUserName);
        _chatService.CreateRoom(cleanRoomName, new[] { cleanUserName });

        return RedirectToAction("Room", "Chat", new { roomName = cleanRoomName });
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("CurrentUser");
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
