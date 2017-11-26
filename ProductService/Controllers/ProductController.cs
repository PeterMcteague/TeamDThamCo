using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Controllers
{
    [Produces("application/json")]
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private readonly ProductContext _context;

        public ProductController(ProductContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This lists all the products
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("",Name ="List all products")]
        public async Task<IActionResult> getProducts()
        {
            if (!_context.Products.Any())
            {
                return NotFound();
            }
            else
            {
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
        }
    }
}