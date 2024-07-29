using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Migrations;
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

        public async Task<IActionResult> CartMenu()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            string userId = user.Id;
            var userCart = _context.Carts
                .Include(c => c.user)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (userCart == null)
            {
                userCart = new Cart
                {
                    UserId = userId,
                    user = user,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(userCart);
                await _context.SaveChangesAsync();
            }

            var cartViewModel = new CartViewModel
            {
                Email = userCart.user?.Email,
                Avatar = userCart.user?.Avatar,
                CartItems = userCart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Price,
                    Image = ci.Product.Image
                }).ToList()
            };

            return View(cartViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(string selectedPaymentMethod)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { error = "User not logged in" });
            }

            string userId = currentUser.Id;
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
                    Price = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId).Price
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
                Status = "Success"
            };
            _context.Payments.Add(payment);

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Json(new { orderId = order.Id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int? optionId, decimal? optionPrice)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                string userId = user.Id;
                var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        user = user
                    };
                    _context.Carts.Add(cart);
                }

                var product = _context.Products.Include(p => p.ProductOptions).FirstOrDefault(p => p.Id == productId);

                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }

                if (product.ProductOptions.Any())
                {
                    // Product has options; check for item with specific option and price
                    var existingCartItem = cart.CartItems
                        .FirstOrDefault(ci => ci.ProductId == productId && ci.ProductOptionId == optionId
                                              && (optionPrice == null || ci.Price == optionPrice));

                    if (existingCartItem != null)
                    {
                        // If the item exists, increase the quantity
                        existingCartItem.Quantity += 1;
                    }
                    else
                    {
                        // If the item does not exist, add a new item to the cart
                        var cartItem = new CartItem
                        {
                            Cart = cart,
                            ProductId = productId,
                            ProductOptionId = optionId,
                            Quantity = 1,
                            Price = optionPrice ?? product.Price
                        };
                        _context.CartItems.Add(cartItem);
                    }
                }
                else
                {
                    // Product does not have options; check for item without option
                    var existingCartItem = cart.CartItems
                        .FirstOrDefault(ci => ci.ProductId == productId && ci.ProductOptionId == null);

                    if (existingCartItem != null)
                    {
                        // If the item exists, increase the quantity
                        existingCartItem.Quantity += 1;
                    }
                    else
                    {
                        // If the item does not exist, add a new item to the cart
                        var cartItem = new CartItem
                        {
                            Cart = cart,
                            ProductId = productId,
                            ProductOptionId = null,
                            Quantity = 1,
                            Price = product.Price
                        };
                        _context.CartItems.Add(cartItem);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product added to cart." });
            }
            return Json(new { success = false, message = "User not found." });
        }




        public async Task<IActionResult> CheackOut()
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id;

            var userCart = _context.Carts
                .Include(c => c.user)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.ProductOptions)
                .FirstOrDefault(c => c.UserId == userId);

            if (userCart == null)
            {
                return NotFound();
            }

            // Prepare CartViewModel with product options
            var cartViewModel = new CartViewModel
            {
                Email = userCart.user.Email,
                Avatar = userCart.user.Avatar,
                CartItems = userCart.CartItems.Select(ci =>
                {
                    var productOptions = ci.Product.ProductOptions
                        .Where(po => po.OptionPrice == ci.Price)
                        .Select(po => new ProductOptionViewModel
                        {
                            
                            OptionName = po.OptionName,
                            OptionPrice = po.OptionPrice
                        }).ToList();

                    return new CartItemViewModel
                    {
                        CartItemId = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        Quantity = ci.Quantity,
                        Price = ci.Price,
                        Image = ci.Product.Image,
                        ProductOptions = productOptions // Add ProductOptions to the CartItemViewModel
                    };
                }).ToList()
            };

            // Prepare ProductOptions dictionary if needed for other purposes
            var productOptionsDict = userCart.CartItems
                .SelectMany(ci => ci.Product.ProductOptions)
                .Where(po => userCart.CartItems.Any(ci => ci.Price == po.OptionPrice))
                .Distinct()
                .ToDictionary(po => po.Id, po => po.OptionName);

            ViewBag.ProductOptions = productOptionsDict;

            return View(cartViewModel);
        }



    }

}
