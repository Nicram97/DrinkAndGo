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
    [Authorize]
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

            ViewData["Orders"] = _orders;

            return View(_orders);
        }

        public ViewResult Details(int orderId)
        {
            var selectedOrder = _appDbContext.Orders.FirstOrDefault(p => p.OrderId == orderId);
            string isSent = "W trakcie realizacji";
            
            if ((DateTime.Now - selectedOrder.OrderPlaced).TotalDays >= 3 ) {
                isSent = "Zrealizowane";
            }

            List<Product> orderDetails = _appDbContext.OrderDetails.Join(
                _appDbContext.Drinks,
                      orderDetail => orderDetail.DrinkId,
                      drink => drink.DrinkId,
                      (orderDetail, drink) => new Product(
                          orderDetail.OrderId,
                          orderDetail.DrinkId,
                          drink.Name,
                          orderDetail.Price,
                          selectedOrder.OrderTotal,
                          isSent,
                          orderDetail.Amount
                      )
                ).Where(detail => detail.OrderId == orderId).ToList();

            if (selectedOrder == null || orderDetails == null)
            {
                return View("~/Views/Error/Error.cshtml");
            }

            ViewData["Products"] = orderDetails;
            ViewData["isSent"] = isSent;

            return View(selectedOrder);
        }
    }
}