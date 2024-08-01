using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using OnlineOrderingSystem.ViewModels;

namespace OnlineOrderingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> DetailsProducts(int? id, int loadCount = 2)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .ThenInclude(c => c.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var comments = product.Comments.Take(loadCount).ToList();
            ViewBag.TotalComments = product.Comments.Count;
            ViewBag.LoadCount = loadCount;
            ViewBag.Comments = comments;

            return View("DetailsProducts", product);
        }

        [HttpGet]
        public IActionResult GetMoreComments(int productId, int loadCount)
        {
            var comments = _context.Comments
                .Where(c => c.ProductId == productId)
                .Include(c => c.user)
                .Skip(loadCount).Take(4).ToList();

            return Ok(comments.Select(c => new
            {
                c.user.UserName,
                c.Content
            }));
        }

        public IActionResult CreateProduct()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProducts(Product product, IFormFile ImageFile, List<IFormFile> ImageFiles)
        {
            // Handle cover image upload
            if (ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                product.Image = uniqueFileName;
            }

            // Handle additional images upload
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                foreach (var imageFile in ImageFiles)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ProductImages.Add(new ProductImage { ImageUrl = uniqueFileName });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> Wishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var products = await _context.Products
                .Include(p => p.ProductOptions)
                .ToListAsync();

            var userWishlists = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .Include(w => w.User)
                .ToListAsync();

            ViewBag.Products = products;
            ViewBag.UserWishlists = userWishlists;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edite(int id, [Bind("Id,Name,Description,Price,Image,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public IActionResult Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var products = _context.Products
                    .Where(p => p.Name.Contains(query))
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.Image,
                        p.CategoryId,
                        CategoryName = p.Category.Name
                    })
                    .ToList();
                return Ok(products);
            }
            return Ok(Enumerable.Empty<object>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(string Content, int id)
        {
            var product = await _context.Products.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Comment comment1 = new Comment();

            comment1.DatePosted = DateTime.Now;
            comment1.UserId = userId;
            comment1.ProductId = id;
            comment1.Product = product;
            comment1.Content = Content;
            _context.Add(comment1);
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsProducts", "Products", new { id = id });
        }

        [HttpPost]
        public IActionResult ToggleCommentsEnabled(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            product.CommentsEnabled = !product.CommentsEnabled;
            _context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> TopProuct()
        {
            var topProducts = await _context.Wishlists
                .GroupBy(w => w.ProductId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(10)
                .ToListAsync();

            var products = await _context.Products
                .Where(p => topProducts.Contains(p.Id))
                .ToListAsync();

            return View(products);
        }

       

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
