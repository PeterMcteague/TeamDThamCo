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

        // GET: <controller>
        public IActionResult Index()
        {
            // Add connection string to viewbag
            var OrderString = configuration.GetConnectionString("OrderService") + "orders/checkoutdetails";
            ViewBag.OrderConnection = OrderString;

#if DEBUG
            var userId = "test-id-plz-ignore";
#else
            // Not sure where we're getting this from tbh , either customer deets service or from token
            var userId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();
#endif
            var basket = _context.Baskets.Where(b => b.buyerId == userId).ToList();
            return View(basket);
        }
        
        public IActionResult Delete(int id)
        {
            var toDelete = _context.Baskets.Where(b => b.id == id).FirstOrDefault();
            if (toDelete != null)
            {
                _context.Baskets.Remove(toDelete);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}