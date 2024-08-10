using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using OnlineOrderingSystem.Services;
using OnlineOrderingSystem.ViewModels;

namespace OnlineOrderingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

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

        public IActionResult UserSitting()
        {

            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult UserEdite(string Id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = _userManager.GetRolesAsync(user).Result;
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.UserRoles = userRoles;
            return View(user);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edite(string id, User user, string[] selectedRoles)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update user details.");
                    return View(user);
                }

                var userRoles = await _userManager.GetRolesAsync(existingUser);

                var rolesToAdd = selectedRoles.Except(userRoles);
                result = await _userManager.AddToRolesAsync(existingUser, rolesToAdd);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add selected roles to user.");
                    return View(user);
                }

                var rolesToRemove = userRoles.Except(selectedRoles);
                result = await _userManager.RemoveFromRolesAsync(existingUser, rolesToRemove);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove unselected roles from user.");
                    return View(user);
                }

                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(user);
            }
        }

        public IActionResult AdminSearch(ProductSearchViewModel model)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(model.Name))
            {
                query = query.Where(p => p.Name.Contains(model.Name));
            }

            if (model.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= model.MinPrice.Value);
            }

            if (model.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= model.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(model.Category))
            {
                query = query.Where(p => p.Category.Name.Contains(model.Category));
            }

            model.Results = query.ToList();
            return View(model);
        }




        public async Task<IActionResult> SalesAnalytics()
        {
            var completeOrders = await _context.Orders
                .Where(o => o.Status == "Complete")
                .ToListAsync();

         

            return View(completeOrders);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateDiscounts(int[] categoryIds, decimal[] discountPercentages)
        {
            for (int i = 0; i < categoryIds.Length; i++)
            {
                var categoryId = categoryIds[i];
                var discountPercentage = discountPercentages[i];

                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.CategoryId == categoryId);

                if (discount != null)
                {
                    discount.DiscountPercentage = discountPercentage;
                }
                else
                {
                    _context.Discounts.Add(new Discount
                    {
                        Name = $"Discount for Category {categoryId}",
                        DiscountPercentage = discountPercentage,
                        CategoryId = categoryId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
       

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }

    
}

