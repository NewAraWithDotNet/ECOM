using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using OnlineOrderingSystem.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using OnlineOrderingSystem.Extensions;
using System.IO;

namespace OnlineOrderingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("HomePage");
            }
            var model = new SliderModel();
            model.OnGet();
            return View(model);
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
            var topProducts = await _context.Wishlists
                  .GroupBy(w => w.ProductId)
                  .OrderByDescending(g => g.Count())
                  .Select(g => g.Key)
                  .Take(10)
                  .ToListAsync();

            var products = await _context.Products
                .Where(p => topProducts.Contains(p.Id))
                .ToListAsync();
             ViewBag.categorieslist = await _context.Categories.ToListAsync();
            
            return View(products);
        }
        public async Task<IActionResult> Shope(int? catid)
        {        
			var categories = _context.Categories.Include(c => c.Products).ToList();
            ViewBag.catid=catid;
			return View(categories);
		}








		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult UserP()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                ViewBag.Email = user.Email;
                ViewBag.UserName = user.UserName;
                ViewBag.Avatar = user.Avatar;
            }

            return View();
        }
        public IActionResult Slider()
        {
            var model = new SliderModel();
            model.OnGet();
            ViewBag.Slider = model;
            return View();
        }
        // My Account 
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Unable to load user.");
            }

            var model = new ManageViewModel
            {
                CurrentUserName = user.UserName,
                CurrentEmail = user.Email,
                CurrentAvatar = user.Avatar,
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUsername(ManageViewModel model)
        {
            
                var success = await _userManager.UpdateUsernameAsync(model.CurrentUserName, model.NewUsername);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update username.");
                }
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(ManageViewModel model)
        {
            
                var success = await _userManager.UpdateEmailAsync(model.CurrentEmail, model.NewEmail);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update email.");
                }
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ManageViewModel model)
        {
            
                var success = await _userManager.UpdatePasswordAsync(model.CurrentUserName, model.CurrentPassword, model.NewPassword);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update password.");
                }
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(ManageViewModel model)
        {
           
                string avatarPath = null;

                if (model.NewAvatar != null && model.NewAvatar.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    var fileName = Path.GetFileName(model.NewAvatar.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewAvatar.CopyToAsync(stream);
                    }

                    avatarPath = $"/uploads/{fileName}";
                }

                var success = await _userManager.UpdateAvatarAsync(model.CurrentUserName, avatarPath);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update avatar.");
                }
            
            return RedirectToAction("Index");
        }
    }
}
