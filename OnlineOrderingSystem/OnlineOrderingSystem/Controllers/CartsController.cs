using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using OnlineOrderingSystem.ViewModels;

namespace OnlineOrderingSystem.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id;
            {
                var userCart = _context.Carts
                    .Include(c => c.user)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == userId);

                if (userCart == null)
                {
                    return NotFound();
                }

                var cartViewModel = new CartViewModel
                {
                    Email = userCart.user.Email,
                    Avatar = userCart.user.Avatar,
                    CartItems = userCart.CartItems.Select(ci => new CartItemViewModel
                    {
                        CartItemId = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price,
                        Image = ci.Product.Image
                    }).ToList()
                };

                return View(cartViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string selectedPaymentMethod)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            string userId = currentUser.Id;

            if (currentUser == null)
            {

                return RedirectToAction("Login", "Account");
            }


            var order = new Order { UserId = userId, OrderDate = DateTime.Now, Status = "Pending" };

            var cartItems = _context.CartItems.Where(ci => ci.Cart.UserId == currentUser.Id).ToList();
            _context.Orders.Add(order);
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    Order = order,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = _context.Products.FirstOrDefault(P => P.Id == cartItem.ProductId)!.Price!
                };
                _context.OrderItems.Add(orderItem);

            }


            order.TotalPrice = order.OrderItems.Sum(oi => oi.Quantity * oi.Price);

            var payment = new Payment
            {
                Order = order,
                Amount = order.TotalPrice,
                PaymentDate = DateTime.Now,
                PaymentMethod = selectedPaymentMethod,
                Status = "Success",

            };
            _context.Payments.Add(payment);



            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("Index", "Orders", new { orderId = order.Id });
        }




        [Authorize]
        public async Task<IActionResult> AddToCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                string userId = user.Id;

                var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        user = user
                    };
                    _context.Carts.Add(cart);
                }

                var cartItem = _context.CartItems
                    .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == id);

                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                }
                else
                {
                    cartItem = new CartItem
                    {
                        Cart = cart,
                        ProductId = id,
                        Quantity = 1
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product added to cart." });
            }
            return Json(new { success = false, message = "User not found." });
        }


        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id;
            {
                var userCart = _context.Carts
                    .Include(c => c.user)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == userId);

                if (userCart == null)
                {
                    return NotFound();
                }

                var cartViewModel = new CartViewModel
                {
                    Email = userCart.user.Email,
                    Avatar = userCart.user.Avatar,
                    CartItems = userCart.CartItems.Select(ci => new CartItemViewModel
                    {
                        CartItemId = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price,
                        Image = ci.Product.Image
                    }).ToList()
                };

                return View(cartViewModel);
            }
        }



    }

}
