using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BasketService.Models;
using System.Diagnostics;
using BasketService.Data;
using Microsoft.AspNetCore.Authorization;

namespace BasketService.Controllers
{
    public class BasketController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly BasketContext _context;

        public BasketController(IConfiguration configuration, BasketContext context)
        {
            this.configuration = configuration;
            this._context = context;
        }

        //[Authorize]
        public IActionResult Index()
        {
            // Add connection string to viewbag
            ViewBag.OrderConnection = configuration.GetConnectionString("OrderService");

#if DEBUG
            var userId = "test-id-plz-ignore";
#else
            var userId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();
#endif
            return View(_context.Baskets.Where(b => b.buyerId == userId).ToList());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}