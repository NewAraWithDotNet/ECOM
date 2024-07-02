using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using OnlineOrderingSystem.ViewModels;

namespace OnlineOrderingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
             _userManager = userManager;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
          ViewBag.Categories = categories;

            return View();
        }
        public IActionResult AdminProducts()
        {
            var Products = _context.Products.ToList();
            ViewBag.Products = Products;

            return View();
        }
    }
}
