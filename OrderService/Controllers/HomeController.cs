using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// A MVC controller for returning a home index page

namespace OrderService.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// MVC controller method for returning index
        /// </summary>
        /// <returns>views/home/index.cshtml</returns>
        // GET: /<controller>/
        [Authorize("read:order")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
