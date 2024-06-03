using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks; // Add this using statement for async methods

namespace OnlineOrderingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager; // Add UserManager dependency
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager; // Initialize UserManager
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomePage));
            }

            return View();
        }

        public async Task<IActionResult> HomePage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                ViewBag.Email = user.Email;
                ViewBag.UserName = user.UserName;
                ViewBag.Avatar = user.Avatar;
            }
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
