using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DrinkAndGo.Data;
using DrinkAndGo.Data.interfaces;
using DrinkAndGo.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DrinkAndGo.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;
        IEnumerable<Order> _orders;

        public OrderHistoryController(IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _appDbContext = appDbContext;
        }
        public IActionResult Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityUser user = (IdentityUser)_appDbContext.Users.Find(userId);

            _orders = _appDbContext.Orders.Where(order => order.UserId == user.Id);

            return View(_orders);
        }
    }
}