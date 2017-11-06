using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Produces("application/json")]
    [Route("api/OrdersAPI")]
    public class OrdersAPIController : Controller
    {
        private readonly OrderServiceContext _context;

        public OrdersAPIController(OrderServiceContext context)
        {
            _context = context;
        }

        // GET: api/OrdersAPI
        [HttpGet]
        public IEnumerable<Order> GetOrder()
        {
            return _context.Orders;
        }

        // GET: api/OrdersAPI/5
        [HttpGet("{id}", Name = "Get orders by buyer ID")]
        public async Task<IActionResult> GetOrder([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.buyerId == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/OrdersAPI/Products/5
        [HttpGet("Products/{id}", Name = "Get products in order by buyer ID")]
        public async Task<IActionResult> GetProductsOrder([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.buyerId == id);

            if (order == null)
            {
                return NotFound();
            }
            if (order.products.ToList().Count() == 0)
            {
                return NoContent();
            }
            else
            {
                var listOfProducts = new List<OrderItem>();
                foreach (OrderItem i in order.products)
                {
                    listOfProducts.Add(i);
                }
                return Ok(listOfProducts.ToList());
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.id == id);
        }
    }
}