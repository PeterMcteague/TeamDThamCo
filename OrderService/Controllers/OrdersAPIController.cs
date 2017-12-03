﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using OrderService.Data;
using Hangfire;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using OrderService.Clients;
using Microsoft.AspNetCore.Authorization;

// A controller for the orders API

namespace OrderService.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders/")]
    public class OrdersAPIController : Controller
    {
        private readonly OrderServiceContext _context;
        private string invoiceJobId;

        /// <summary>
        /// Constructor for the API controller
        /// </summary>
        /// <param name="context">An OrderServiceContext object for the orders database</param>
        public OrdersAPIController(OrderServiceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// A helper function which gets items in a order
        /// </summary>
        /// <param name="orderId">The orderId to get orderItems for</param>
        /// <returns>Returns the orderItems for that order in a List</returns>
        private List<OrderItem> getItems(int orderId)
        {
            return _context.OrderItems.Where(b => b.active == true && b.orderId == orderId).ToList();
        }

        /// <summary>
        /// A helper function that creates an invoice for any uninvoiced orders
        /// </summary>
        /// <returns>Whether the invoice was successfully created</returns>
        private bool createInvoice()
        {
            // Get uninvoiced
            var orders = _context.Orders.Where(b => b.invoiced == false).ToList();
            bool success = createInvoice(orders).IsCompleted;
            return success;
        }

        /// <summary>
        /// Creates an invoice from a list of orders by sending invoices through a messaging API and if succesful updates the dispatch and invoice status of the order.
        /// </summary>
        /// <param name="orders">A list of orders to invoice</param>
        /// <returns>Returns whether invoicing these orders was succesful</returns>
        private async Task<bool> createInvoice(List<Order> orders)
        {
            // Send invoice
            try
            {
                MessageSender sender = new MessageSender(_context);
                HttpResponseMessage response = await sender.SendOrderInvoice(orders);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false; //We don't want to do anything if we can't send to the messaging service. The job remains in the queue so it should try again every 5 minutes if it exists.
            }
            // Mark invoiced and Cancel the recurring job and dispatch it
            BackgroundJob.Delete(invoiceJobId);
            foreach (Order o in orders)
            {
                o.invoiced = true;
                _context.Update(o);
                await UpdateOrderDispatch(o.id, true);
            }
            return true;
        }

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <response code="200">Returns the orders</response>
        /// <response code="204">If not any orders</response>  
        /// <response code="400">If parameters are invalid</response>
        [Authorize("read:order")]
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
        /// <response code="204">If not any orders by buyerId</response>  
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any orders</response>  
        [Authorize("read:order")]
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
        /// <response code="400">If parameters are invalid</response>
        /// <response code="404">If not any products ordered</response>  
        [Authorize("read:order")]
        [HttpGet("Products/Get", Name = "Get all products ordered")]
        public async Task<IActionResult> GetOrderItems()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.OrderItems.Any())
            {
                return NotFound("No order items found");
            }

            var products = await _context.OrderItems.Where(b => b.active == true).ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Gets all products in an order.
        /// </summary>
        /// <param name="orderid">The ID of the order to get products by.</param>  
        /// <response code="200">Returns the products</response>
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any products with that orderID</response>  
        [Authorize("read:order")]
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
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any products ordered by that buyer</response>  
        [Authorize("read:order")]
        [HttpGet("Products/Get/buyerid={buyerid}", Name = "Get products ordered by buyer ID")]
        public async Task<IActionResult> GetOrderItemsByBuyerID([FromRoute] string buyerid)
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
        /// Updates an orders address
        /// </summary>
        /// <param name="id">ID of order to update.</param>
        /// <param name="address">Address</param>
        /// <response code="200">Returns the new order</response>
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any orders with that id</response>  
        [Authorize("create:order")]
        [HttpPut("Update/id={id}&address={address}", Name = "Update order address")]
        public async Task<IActionResult> UpdateOrderAddress([FromRoute] int id, [FromRoute] string address)
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
                _context.Update(order);
                await _context.SaveChangesAsync();
                return Ok(order);
            }
        }

        /// <summary>
        /// Updates an orders dispatch status
        /// </summary>
        /// <param name="id">ID of order to update.</param>
        /// <param name="dispatched">Dispatch boolean</param>
        /// <response code="200">Returns the new order</response>
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any products ordered by that buyer</response>  
        [Authorize("create:order")]
        [HttpPut("Update/id={id}&dispatched={dispatched}", Name = "Update order dispatch status")]
        public async Task<IActionResult> UpdateOrderDispatch([FromRoute] int id, [FromRoute] bool dispatched)
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
            if (!_context.Orders.Any(b => b.active == true && b.id == id && b.paid == false))
            {
                return BadRequest("Order must be paid before it is dispatched");
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(m => m.id == id);
                order.dispatched = dispatched;
                _context.Update(order);
                await _context.SaveChangesAsync();
                _context.Dispatches.Add(new Dispatch { orderId = id });
                return Ok(order);
            }
        }

        /// <summary>
        /// Updates an orders paid status , to be called after a succesful payment response is recieved
        /// </summary>
        /// <param name="id">ID of order to update.</param>
        /// <param name="paid">Paid boolean</param>
        /// <response code="200">Returns the new order</response>
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any products ordered by that buyer</response>  
        [Authorize("create:order")]
        [HttpPut("Update/id={id}&paid={paid}", Name = "Update order paid status")]
        public async Task<IActionResult> UpdateOrderPaid([FromRoute] int id, [FromRoute] bool paid, bool sendInvoice)
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
                if (paid)
                {
                    var orderItems = getItems(order.id);
                    if (orderItems.Sum(item => item.cost) > 50.00m)
                    {
                        if (await createInvoice(new List<Order> { order }))
                        {
                            order.paid = paid;
                            _context.Update(order);
                        }
                    }
                    else
                    {
                        var unInvoicedOrders = _context.Orders.Where(b => b.buyerId == order.buyerId && b.invoiced == false).ToList();
                        var totalCost = 0.00m;
                        foreach (Order o in unInvoicedOrders)
                        {
                            var items = getItems(o.id);
                            totalCost += items.Sum(item => item.cost);
                        }
                        if (totalCost > 50m)
                        {
                            if(await createInvoice(unInvoicedOrders))
                            {
                                order.paid = paid;
                                _context.Update(order);
                            }
                        }
                        else
                        {
                            invoiceJobId = BackgroundJob.Schedule(() => createInvoice(), TimeSpan.FromMinutes(5));
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(order);
            }
        }

        /// <summary>
        /// Updates an order item
        /// </summary>
        /// <param name="orderid">The order to update</param>  
        /// <param name="quantity">The new item quantity</param>  
        /// <response code="200">Successfully updated, returns new data</response>
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any orders with that orderID that aren't dispatched</response>  
        [Authorize("create:order")]
        [HttpPut("Products/Update/id={id}quantity={quantity}", Name = "Update order item quantity")]
        public async Task<IActionResult> UpdateOrderItemQuantity([FromRoute] int id, [FromRoute] int quantity)
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
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If not any orders with that orderID that aren't dispatched</response>
        [Authorize("create:order")]
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
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="404">If product isn't found in any orders or is already dispatched</response>  
        [Authorize("create:order")]
        [HttpPut("Products/Delete/productId={productId}", Name = "Delete product from order if not dispatched")]
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
        /// <response code="200">The added order</response>
        /// <response code="400">If parameters are invalid</response>  
        [Authorize("create:order")]
        [HttpPost("Add/buyerId={buyerId}&address={address}", Name = "Add an order")]
        public async Task<IActionResult> AddOrder([FromRoute] string buyerId, [FromRoute] string address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var itemToAdd = new Order {orderDate = DateTime.Now , address = address , buyerId = buyerId , paid = false, dispatched = false , invoiced = false , active = true};
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
        /// <response code="200">The added order item</response>
        /// <response code="400">If parameters are invalid</response>  
        /// <response code="400">If not any orders or the entered orderId is invalid</response>  
        [Authorize("create:order")]
        [HttpPost("Products/Add/orderId={orderId}&productId={productId}&name={name}&quantity={quantity}&cost={cost}", Name = "Add an orderitem")]
        public async Task<IActionResult> AddOrderItem([FromRoute] int orderId, [FromRoute] int productId, [FromRoute] string name, [FromRoute] int quantity, [FromRoute] decimal cost)
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