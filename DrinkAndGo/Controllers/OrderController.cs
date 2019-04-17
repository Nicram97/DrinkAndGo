using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DrinkAndGo.Data;
using DrinkAndGo.Data.interfaces;
using DrinkAndGo.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DrinkAndGo.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ShoppingCart _shoppingCart;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;

        public OrderController(IOrderRepository orderRepository, ShoppingCart shoppingCart, IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityUser user = (IdentityUser) _appDbContext.Users.Find(userId);

            ViewData["user"] = user;

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Checkout(Order order)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            decimal total = _shoppingCart.GetShoppingCartTotal();
            order.OrderTotal = total;
            _shoppingCart.ShoppingCartItems = items;
            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your card is empty, add some drinks first");
            }

            if (ModelState.IsValid)
            {
                _orderRepository.CreateOrder(order);
                _shoppingCart.ClearCart();
                return RedirectToAction("CheckoutComplete");
            }
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityUser user = (IdentityUser)_appDbContext.Users.Find(userId);

            ViewData["user"] = user;
            return View(order);
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Thanks for your order! :) ";
            return View();
        }
    }
}