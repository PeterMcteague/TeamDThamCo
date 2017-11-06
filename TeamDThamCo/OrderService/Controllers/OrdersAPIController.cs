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

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Orders.Any())
            {
                return NotFound();
            }

            return Ok(_context.Orders);
        }

        // GET: api/Orders/5
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

        // GET: api/Orders/Products
        [HttpGet("Products/{id}", Name = "Get all products ordered")]
        public async Task<IActionResult> GetProductsOrdered()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound();
            }

            return Ok(_context.OrderItems);
        }

        // GET: api/Orders/Products/5
        [HttpGet("Products/{id}", Name = "Get products in order by order ID")]
        public async Task<IActionResult> GetProductsInOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orders = _context.OrderItems.Where(m => m.orderId == id);

            if (!orders.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(orders);
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.id == id);
        }
    }
}