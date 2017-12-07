using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using BasketFrontend.Clients;
using BasketFrontend.Models;
using Newtonsoft.Json;

namespace BasketFrontend.Controllers
{
    public class BasketController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly BasketClient basketClient;
        private string userId;

        public BasketController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.basketClient = new BasketClient(configuration);

#if DEBUG
            this.userId = "test-id-plz-ignore";
#else
            // Not sure where we're getting this from tbh , either customer deets service or from token
            this.userId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();
#endif
        }

        // GET: <controller>
        public async Task<IActionResult> Index()
        {
            // Add connection string to viewbag
            var OrderString = configuration.GetConnectionString("OrderService") + "orders/checkoutdetails";
            ViewBag.OrderConnection = OrderString;

            // Get basket
            var basket = await basketClient.GetBasketAsync(userId);
            return View(basket);
        }
        
        public IActionResult Delete(int id)
        {
            var response = basketClient.DeleteSingleBasket(userId,id);
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}