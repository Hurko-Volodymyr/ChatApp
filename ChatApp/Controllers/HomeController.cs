using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;
using ChatApp.Hubs;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatContext _context;

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext, ChatContext context)
        {
            _logger = logger;
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.Messages
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
            ViewBag.Messages = messages;
            return View();
        }

        public async Task<IActionResult> SendMessage(string sender, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Message cannot be empty");
            }

            sender ??= "Anonymous";

            var msg = new Message
            {
                Sender = sender,
                Content = message,
                Timestamp = DateTime.UtcNow,
                Sentiment = "neutral"
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", sender, message);

            return Ok();
        }

        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
