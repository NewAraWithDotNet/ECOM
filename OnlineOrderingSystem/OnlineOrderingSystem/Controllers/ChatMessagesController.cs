using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

namespace OnlineOrderingSystem.Controllers
{
    public class ChatMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ChatMessagesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var conversations = _context.Conversations
                                        .Include(c => c.User1)
                                        .Include(c => c.User2)
                                        .ToList();

            ViewBag.Conversations = conversations;
            ViewBag.LoggedInUserId = loggedInUserId;

            return View();
        }


        public async Task<IActionResult> Conversation(string user1 , string user2)
        {
            var userData1 = await _userManager.FindByNameAsync(user1);

            var userData2 = await _userManager.FindByNameAsync(user2);

            var conversation = await _context.Conversations
           .Include(c => c.Messages)
           .FirstOrDefaultAsync(c =>
               (c.User1Id == userData1.Id && c.User2Id == userData2.Id) ||
               (c.User1Id == userData2.Id && c.User2Id == userData1.Id));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = userData1.Id,
                    User1 = userData1,
                    User2Id = userData2.Id,
                    User2 = userData2,
                    Messages = new List<ChatMessage>()
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }
            return View(conversation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation(string user1Id, string user2Id)
        {
            var conversation = new Conversation
            {
                User1Id = user1Id,
                User2Id = user2Id
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Conversation), new { id = conversation.Id });
        }
    }
}
