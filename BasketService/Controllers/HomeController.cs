using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// A controller for a simple home page

namespace BasketService
{
    public class HomeController : Controller
    {
        /// <summary>
        /// MVC controller method for returning index
        /// </summary>
        /// <returns>views/home/index.cshtml</returns>
        // GET: /<controller>/
        [Authorize("read:basket")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
