using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using OnlineOrderingSystem.ViewModels;

namespace OnlineOrderingSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public IActionResult Index()
        {
            var orders = _context.Orders.Include(o => o.User).ToList();
            return View(orders);
        }
        // GET: Orders/Details/5
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
                                .Include(o => o.User)
                                .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.Product)
                                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderViewModel
            {
                Order = order,
                OrderItems = order.OrderItems
            };

            return View(viewModel);
        }
        // GET: Orders/Create


        public async Task<IActionResult> MyOrderAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");             }

            string userId = currentUser.Id;
            var orders = _context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                        .Where(o => o.UserId == userId)
                        .ToList();

            return View(orders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.Include(o => o.User).Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
             _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        [HttpGet]
        public async Task<IActionResult> UserOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems) 
                .ToListAsync();

            return View(orders);
        }
    }
}
