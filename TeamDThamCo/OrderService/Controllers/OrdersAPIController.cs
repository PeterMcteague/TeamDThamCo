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
    ///<Summary>
    /// A controller for the orders API.
    /// TODO: Have deletes refund payment.
    ///</Summary>
    [Produces("application/json")]
    [Route("api/Orders/")]
    public class OrdersAPIController : Controller
    {
        private readonly OrderServiceContext _context;

        public OrdersAPIController(OrderServiceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <response code="200">Returns the orders</response>
        /// <response code="400">If not any orders</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("", Name = "Get all orders")]
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

            return Ok(_context.Orders.Where(b => b.active == true));
        }

        /// <summary>
        /// Gets all orders made by a customer.
        /// </summary>
        /// <param name="Buyer ID"></param>  
        /// <response code="200">Returns the orders</response>
        /// <response code="400">If not any orders by buyerId</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("{buyerid}", Name = "Get orders by buyer ID")]
        public async Task<IActionResult> GetOrder([FromRoute] string buyerid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orders = _context.Orders.Where(m => m.buyerId == buyerid && m.active == true);

            if (orders == null)
            {
                return NotFound();
            }

            return Ok(orders);
        }

        /// <summary>
        /// Gets all products ordered.
        /// </summary>
        /// <response code="200">Returns the products</response>
        /// <response code="400">If not any products ordered</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("Products", Name = "Get all products ordered")]
        public async Task<IActionResult> GetProductsOrdered()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = _context.OrderItems.Where(b => b.active == true);

            if (!products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        /// <summary>
        /// Gets all products in an order.
        /// </summary>
        /// <param name="Order ID"></param>  
        /// <response code="200">Returns the products</response>
        /// <response code="400">If not any products with that orderID</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("Products/orderid={orderid}", Name = "Get products in order by order ID")]
        public async Task<IActionResult> GetProductsOrdered([FromRoute] int orderid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = _context.OrderItems.Where(b => b.active == true && b.orderId == orderid);

            if (!products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        /// <summary>
        /// Gets all products ordered by a customer.
        /// </summary>
        /// <param name="Buyer ID"></param>  
        /// <response code="200">Returns the products</response>
        /// <response code="400">If not any products ordered by that buyer</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("Products/buyerid={buyerid}", Name = "Get products ordered by buyer ID")]
        public async Task<IActionResult> GetProductsOrdered([FromRoute] string buyerid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var listOfOrderIdsByBuyer = _context.Orders.Where(r => r.buyerId == buyerid && r.active).Select(r => r.id);
            var listOfProducts = _context.OrderItems.Where(r => listOfOrderIdsByBuyer.Contains(r.orderId) && r.active == true);

            if (!listOfProducts.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(listOfProducts);
            }
        }

        /// <summary>
        /// Cancels an order if not dispatched.
        /// </summary>
        /// <param name="Order ID"></param>  
        /// <response code="200">Successfully deleted</response>
        /// <response code="400">If not any orders with that orderID that aren't dispatched</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpPut("Delete/orderId={orderid}", Name = "Delete order by orderID if not dispatched")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int orderid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orders = _context.Orders.SingleOrDefault(m => m.id == orderid && m.active && m.dispatched == false);

            if (orders == null)
            {
                return NotFound();
            }
            else
            {
                var products = _context.OrderItems.Where(m => m.orderId == orders.id);
                orders.active = false;
                foreach (OrderItem product in products)
                {
                    product.active = false;
                }
                _context.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Removes an item from an order.
        /// </summary>
        /// <param name="Product ID"></param>  
        /// <response code="200">Succesfully deleted from order</response>
        /// <response code="400">If product isn't found in any orders or is already dispatched</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpPut("Delete/productId={productId}", Name = "Delete product from order if not dispatched")]
        public async Task<IActionResult> DeleteProductFromOrder([FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = _context.OrderItems.SingleOrDefault(m => m.id == productId && m.active);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var order = _context.Orders.SingleOrDefault(m => m.id == products.orderId);
                if (!order.dispatched)
                {
                    products.active = false;
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound("Order already dispatched");
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.id == id);
        }
    }
}