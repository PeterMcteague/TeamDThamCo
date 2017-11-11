using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    public class CheckoutController : Controller
    {
        // Post: Checkout/Index
        [HttpPost]
        public ActionResult Details(CheckoutItem id)
        {
            return View();
        }
    }
}