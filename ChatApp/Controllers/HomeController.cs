using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using ChatApp.Hubs;
using ChatApp.Abstractions;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService;

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext, IChatService chatService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _chatService = chatService;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _chatService.GetAllMessagesAsync();
            ViewBag.Messages = messages;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> SendMessage(string sender, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Message cannot be empty");
            }

            sender ??= "Anonymous";

            var msg = await _chatService.AddMessageAsync(sender, message);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", msg.Sender, msg.Content);
            return Ok();
        }

        public async Task<IActionResult> DeleteMessage(int id)
        {
            var result = await _chatService.DeleteMessageAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

