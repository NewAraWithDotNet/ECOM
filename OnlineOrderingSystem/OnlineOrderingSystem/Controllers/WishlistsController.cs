using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;

namespace OnlineOrderingSystem.Controllers
{
    public class WishlistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistsController(ApplicationDbContext context)
        {
               _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlists = await _context.Wishlists
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(wishlists);
        }


        public async Task<IActionResult> ToggleWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingWishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingWishlist != null)
            {
                _context.Wishlists.Remove(existingWishlist);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isAdded = false, message = "Product removed from wishlist successfully." });
            }
            else
            {
                var wishlist = new Wishlist
                {
                    UserId = userId,
                    ProductId = productId
                };

                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isAdded = true, message = "Product added to wishlist successfully." });
            }
        }



    }
}
