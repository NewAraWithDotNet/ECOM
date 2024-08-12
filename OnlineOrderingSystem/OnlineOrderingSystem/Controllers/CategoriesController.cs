using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
namespace OnlineOrderingSystem.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;


		public CategoriesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}
		public async Task<IActionResult> CategoriesList()
		{
			var categorieslist = await _context.Categories.ToListAsync();
			return View(categorieslist);
		}

		public async Task<IActionResult> Index()
		{
			var catList = await _context.Categories.ToListAsync();
			return View(catList);
		}

		public ActionResult Details(int id)
		{
			var category = _context.Categories.Include("Products")
							 .FirstOrDefault(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var userWishlists = _context.Wishlists
															 .Where(w => w.UserId == userId)
															 .Include(w => w.Product)
															 .Include(w => w.User)
															 .ToListAsync();

				ViewBag.UserWishlists = userWishlists;


				return PartialView("Details", category);
			}
			return View(category);
		}
	

		// GET: Categories/Create
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Category category)
		{

			string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await category.ImageFile.CopyToAsync(fileStream);
			}

			category.Image = uniqueFileName;

			_context.Add(category);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

			return View(category);
		}
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image")] Category category)
		{
			if (id != category.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(category);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CategoryExists(category.Id))
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
			return View(category);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories
				.FirstOrDefaultAsync(m => m.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category != null)
			{
				_context.Categories.Remove(category);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}



		public IActionResult CategoryProduct(int id)
		{
			var category = _context.Categories
				.Include(c => c.Products)
				.FirstOrDefault(c => c.Id == id);

			if (category == null)
			{
				return NotFound();
			}

			var FiveProducts = category.Products.Take(5).ToList();

			return PartialView("Details", FiveProducts);
		}
		public IActionResult ShopeProduct(int categoryId)
		{
			var products = _context.Products.Where(p => p.CategoryId == categoryId).ToList();
			return View(products); // Make sure this matches IEnumerable<Product>
		}


		private bool CategoryExists(int id)
		{
			return _context.Categories.Any(e => e.Id == id);
		}
	}
}
