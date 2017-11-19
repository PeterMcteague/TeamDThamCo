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
            if (!_context.Orders.Where(b => b.active == true).Any())
            {
                return NoContent();
            }

            var orders = await _context.Orders.Where(b => b.active == true).ToListAsync();
            return Ok(orders);
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
            if (!_context.Orders.Any())
            {
                return NotFound();
            }
            if (!_context.Orders.Where(b => b.active == true && b.buyerId == buyerid).Any())
            {
                return NoContent();
            }
            var orders = await _context.Orders.Where(m => m.buyerId == buyerid && m.active == true).ToListAsync();
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
            if (!_context.OrderItems.Any())
            {
                return NotFound();
            }

            var products = await _context.OrderItems.Where(b => b.active == true).ToListAsync();

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
            if (!_context.OrderItems.Any())
            {
                return NotFound();
            }
            if (!_context.OrderItems.Where(b => b.active == true && b.orderId == orderid).Any())
            {
                return NoContent();
            }

            var products = await _context.OrderItems.Where(b => b.active == true && b.orderId == orderid).ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Gets all products ordered by a customer.
        /// </summary>
        /// <param name="buyerid"></param>
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
            if (!_context.OrderItems.Any())
            {
                return NotFound();
            }
            if (!_context.Orders.Any())
            {
                return NotFound();
            }
            if (!_context.Orders.Where(b => b.active == true && b.buyerId == buyerid).Any())
            {
                return NoContent();
            }

            var listOfOrderIdsByBuyer = await _context.Orders.Where(m => m.buyerId == buyerid && m.active == true).Select(m => m.id).ToListAsync();

            if (!_context.OrderItems.Where(m => listOfOrderIdsByBuyer.Contains(m.orderId) && m.active == true).Any())
            {
                return NoContent();
            }

            var listOfProducts = await _context.OrderItems.Where(m => listOfOrderIdsByBuyer.Contains(m.orderId) && m.active == true).ToListAsync();
            
            return Ok(listOfProducts);
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
            if (!_context.Orders.Any())
            {
                return NotFound();
            }
            if (!_context.Orders.Where(b => b.active == true && b.id == orderid).Any())
            {
                return NoContent();
            }
            else
            {
                var order = _context.Orders.SingleOrDefault(m => m.id == orderid && m.active && m.dispatched == false);
                var products = _context.OrderItems.Where(m => m.orderId == order.id);
                order.active = false;
                foreach (OrderItem product in products)
                {
                    product.active = false;
                }
                await _context.SaveChangesAsync();
                var returnValue = _context.Orders.Where(b => b.active == true);
                return Ok(returnValue);
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
            if (!_context.OrderItems.Any())
            {
                return NotFound();
            }
            if (!_context.OrderItems.Where(b => b.active == true && b.id == productId).Any())
            {
                return NoContent();
            }

            else
            {
                var product = _context.OrderItems.SingleOrDefault(m => m.id == productId && m.active);
                var order = _context.Orders.SingleOrDefault(m => m.id == product.orderId);
                if (!order.dispatched)
                {
                    product.active = false;
                    await _context.SaveChangesAsync();
                    return Ok(product);
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