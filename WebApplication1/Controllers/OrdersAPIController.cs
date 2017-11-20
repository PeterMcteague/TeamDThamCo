﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using OrderService.Data;

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
        [HttpGet("Get", Name = "Get all orders")]
        public async Task<IActionResult> GetOrder()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Orders.Any())
            {
                return NotFound("No orders found");
            }
            if (!_context.Orders.Any(b => b.active == true))
            {
                return NoContent();
            }

            var orders = await _context.Orders.Where(b => b.active == true).ToListAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Gets all orders made by a customer.
        /// </summary>
        /// <param name="buyerid">The buyer to get orders by.</param>  
        /// <response code="200">Returns the orders</response>
        /// <response code="400">If not any orders by buyerId</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("Get/{buyerid}", Name = "Get orders by buyer ID")]
        public async Task<IActionResult> GetOrder([FromRoute] string buyerid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Orders.Any())
            {
                return NotFound("No orders found");
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
        [HttpGet("Products/Get", Name = "Get all products ordered")]
        public async Task<IActionResult> GetOrderItems()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound("No orders found");
            }

            var products = await _context.OrderItems.Where(b => b.active == true).ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Gets all products in an order.
        /// </summary>
        /// <param name="orderid">The ID of the order to get products by.</param>  
        /// <response code="200">Returns the products</response>
        /// <response code="400">If not any products with that orderID</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("Products/Get/orderid={orderid}", Name = "Get products in order by order ID")]
        public async Task<IActionResult> GetOrderItems([FromRoute] int orderid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound("No orderitems found");
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
        /// <param name="buyerid">The buyerid to get order items by.</param>
        /// <response code="200">Returns the products</response>
        /// <response code="400">If not any products ordered by that buyer</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpGet("Products/Get/buyerid={buyerid}", Name = "Get products ordered by buyer ID")]
        public async Task<IActionResult> GetOrderItems([FromRoute] string buyerid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound("No orderitems found");
            }
            if (!_context.Orders.Any())
            {
                return NotFound("No orders found");
            }
            if (!_context.Orders.Any(b => b.active == true && b.buyerId == buyerid))
            {
                return NoContent();
            }

            var listOfOrderIdsByBuyer = await _context.Orders.Where(m => m.buyerId == buyerid && m.active == true).Select(m => m.id).ToListAsync();

            if (!_context.OrderItems.Any(m => listOfOrderIdsByBuyer.Contains(m.orderId) && m.active == true))
            {
                return NoContent();
            }

            var listOfProducts = await _context.OrderItems.Where(m => listOfOrderIdsByBuyer.Contains(m.orderId) && m.active == true).ToListAsync();
            
            return Ok(listOfProducts);
        }

        /// <summary>
        /// Updates an order
        /// </summary>
        /// <param name="id">ID of order to update</param>
        /// <param name="address">Address</param>
        /// <param name="dispatched">Whether the item is dispatched or not</param>
        /// <param name="invoiced">Whether the item is invoiced or not</param>
        /// <returns></returns>
        [HttpPut("Update/Product/id={id}&address={address}&dispatched={dispatched}&invoiced={invoiced}", Name = "Update order properties")]
        public async Task<IActionResult> UpdateOrderItem([FromRoute] int id, [FromRoute] string address, [FromRoute] bool dispatched, [FromRoute] bool invoiced)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Orders.Any())
            {
                return NotFound("No order items found");
            }
            if (!_context.Orders.Any(b => b.active == true && b.id == id))
            {
                return NoContent();
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(m => m.id == id);
                order.address = address;
                order.dispatched = dispatched;
                order.invoiced = invoiced;
                _context.Update(order);
                await _context.SaveChangesAsync();
                return Ok(order);
            }
        }

        /// <summary>
        /// Updates an order item
        /// </summary>
        /// <param name="orderid">The order to update</param>  
        /// <response code="200">Successfully updated, returns new data</response>
        /// <response code="400">If not any orders with that orderID that aren't dispatched</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpPut("Update/Product/id={id}quantity={quantity}", Name = "Update order item properties")]
        public async Task<IActionResult> UpdateOrderItem([FromRoute] int id, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound("No order items found");
            }
            if (!_context.OrderItems.Any(b => b.active == true && b.id == id))
            {
                return NoContent();
            }
            else
            {
                var product = _context.OrderItems.FirstOrDefault(m => m.id == id);
                product.quantity = quantity;
                _context.Update(product);
                await _context.SaveChangesAsync();
                return Ok(product);
            }
        }

        /// <summary>
        /// Cancels an order if not dispatched.
        /// </summary>
        /// <param name="orderid">The order to delete</param>  
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
                return NotFound("No orders found");
            }
            if (!_context.Orders.Any(b => b.active == true && b.id == orderid))
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
                    _context.Update(product);
                }
                _context.Update(order);
                await _context.SaveChangesAsync();
                var returnValue = _context.Orders.Where(b => b.active == true);
                return Ok(returnValue);
            }
        }

        /// <summary>
        /// Removes an item from an order by orderid and productId
        /// </summary>
        /// <param name="id">The id of the product to delete from an order</param>  
        /// <response code="200">Succesfully deleted from order</response>
        /// <response code="400">If product isn't found in any orders or is already dispatched</response>  
        /// <response code="404">If parameters are invalid</response>  
        [HttpPut("Product/Delete/productId={productId}", Name = "Delete product from order if not dispatched")]
        public async Task<IActionResult> DeleteProductFromOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound("No orderitems found");
            }
            if (!_context.OrderItems.Any(b => b.active == true && b.id == id))
            {
                return NotFound("No orderitems found");
            }

            else
            {
                var product = _context.OrderItems.SingleOrDefault(m => m.id == id && m.active);
                var order = _context.Orders.SingleOrDefault(m => m.id == product.orderId);
                if (!order.dispatched)
                {
                    product.active = false;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return Ok(product);
                }
                return Forbid("Order already dispatched");
            }
        }

        /// <summary>
        /// Adds an order. Note items must be added to the order seperately.
        /// </summary>
        /// <param name="buyerId">The id of the buyer</param>
        /// <param name="address">Their address</param>
        /// <returns>Returns the added item.</returns>
        [HttpPost("Add/buyerId={buyerId}&address={address}", Name = "Add an order")]
        public async Task<IActionResult> AddOrder([FromRoute] string buyerId, [FromRoute] string address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var itemToAdd = new Order {orderDate = DateTime.Now , address = address , buyerId = buyerId , dispatched = false , invoiced = false , active = true};
                await _context.Orders.AddAsync(itemToAdd);
                await _context.SaveChangesAsync();
                return Ok(itemToAdd);
            }
        }

        /// <summary>
        /// Adds an item to an order
        /// </summary>
        /// <param name="orderId">The id of the order to add to</param>
        /// <param name="name">The name of the item to add</param>
        /// <param name="quantity">The quantity of the item to add</param>
        /// <param name="cost">The cost of the item to add</param>
        /// <returns></returns>
        [HttpPost("Add/Product/orderId={orderId}&productId={productId}&name={name}&quantity={quantity}&cost={cost}", Name = "Add an orderitem")]
        public async Task<IActionResult> AddOrderItem([FromRoute] int orderId, [FromRoute] int productId, [FromRoute] string name, [FromRoute] int quantity, [FromRoute] double cost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Orders.Any())
            {
                return NotFound();
            }
            if (!_context.Orders.Any(b => b.id == orderId))
            {
                return NotFound();
            }
            else
            {
                var itemToAdd = new OrderItem {orderId = orderId, productId = productId, itemName = name, quantity = quantity, cost = cost, active = true};
                await _context.OrderItems.AddAsync(itemToAdd);
                await _context.SaveChangesAsync();
                return Ok(itemToAdd);
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.id == id);
        }
    }
}