using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using Microsoft.DotNet.MSIdentity;

namespace OnlineOrderingSystem.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Carts.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }
        [Authorize]
        public IActionResult AddToCart(int productId)
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId!=null)
            {
                var cart = _context.Carts.FirstOrDefault(c => c.UserId == int.Parse(userId));

                if (cart == null)
                {
                    cart = new Cart { UserId = int.Parse(userId) };
                    _context.Carts.Add(cart);
                }

                cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = 1 });

                _context.SaveChanges();

            }

            return RedirectToAction("HomePage", "Home");
            
          
        }

        private int GetCurrentUserId()
        {
            return 1; 
        }
    }
}
