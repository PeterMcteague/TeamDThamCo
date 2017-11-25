using InvoiceService.Data;
using InvoiceService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Controllers
{
    [Produces("application/json")]
    [Route("api/Invoices/")]
    public class InvoiceAPIController : Controller
    {
        private readonly InvoiceContext _context;

        public InvoiceAPIController(InvoiceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns>Returns JSON HTTP Response</returns>
        /// <response code="200">Returns the invoices</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If not any invoices</response>  
        [HttpGet("get", Name = "Get all invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Invoices.Any())
            {
                return NotFound();
            }
            var invoices = await _context.Invoices.ToListAsync();
            return Ok(invoices);
        }

        /// <summary>
        /// Get X invoices
        /// </summary>
        /// <returns>Returns JSON HTTP Response</returns>
        /// <response code="200">Returns the invoices</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If not any invoices</response>  
        [HttpGet("get/quantity={quantity}", Name = "Get X invoices")]
        public async Task<IActionResult> GetInvoices([FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Invoices.Any())
            {
                return NotFound();
            }
            var invoices = await _context.Invoices.Take(quantity).ToListAsync();
            return Ok(invoices);
        }

        /// <summary>
        /// Gets invoices by customer ID
        /// </summary>
        /// <param name="customerId">The customers id string</param>
        /// <returns>Returns JSON HTTP Response</returns>
        /// <response code="200">Returns the invoices</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If not any invoices</response>  
        [HttpGet("get/customerId={customerId}", Name = "Get invoices by customerId")]
        public async Task<IActionResult> GetInvoices([FromRoute] string customerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Invoices.Any(b => b.customerId == customerId))
            {
                return NotFound();
            }
            var invoices = await _context.Invoices.Where(b => b.customerId == customerId).ToListAsync();
            return Ok(invoices);
        }

        /// <summary>
        /// Gets invoices by customer ID
        /// </summary>
        /// <param name="customerId">The customers id string</param>
        /// <returns>Returns JSON HTTP Response</returns>
        /// <response code="200">Returns the invoices</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If not any invoices</response>  
        [HttpGet("get/quantity={quantity}&customerId={customerId}", Name = "Get X invoices by customerId")]
        public async Task<IActionResult> GetInvoices([FromRoute] int quantity , [FromRoute] string customerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Invoices.Any(b => b.customerId == customerId))
            {
                return NotFound();
            }
            var invoices = await _context.Invoices.Where(b => b.customerId == customerId).Take(quantity).ToListAsync();
            return Ok(invoices);
        }

        /// <summary>
        /// Gets orders in an invoice by ID
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice</param>
        /// <returns>Response codes, returns OK with the orders if succesful</returns>
        [HttpGet("get/orders/invoiceId={invoiceId}", Name = "Get orders in an invoice by invoiceId")]
        public async Task<IActionResult> GetInvoiceOrders([FromRoute] int invoiceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.InvoiceOrders.Any() || !_context.Invoices.Any() || !_context.InvoiceOrders.Any(b => b.invoiceId == invoiceId))
            {
                return NotFound();
            }
            var orders = await _context.InvoiceOrders.Where(b => b.invoiceId == invoiceId).ToListAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Gets a quantity of invoice items by invoiceId
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to get invoice items by</param>
        /// <param name="quantity">The number of invoice items to get from that invoiceId</param>
        /// <returns>Response codes, returns OK with the orders if succesful</returns>
        [HttpGet("get/orders/invoiceId={invoiceId}&quantity={quantity}", Name = "Get X orders in an invoice by invoiceId")]
        public async Task<IActionResult> GetInvoiceOrders([FromRoute] int invoiceId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.InvoiceOrders.Any() || !_context.Invoices.Any() || !_context.InvoiceOrders.Any(b => b.invoiceId == invoiceId))
            {
                return NotFound();
            }
            var orders = await _context.InvoiceOrders.Where(b => b.invoiceId == invoiceId).Take(quantity).ToListAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Gets an invoice order containing that orderId , so that you can get the invoiceId from it
        /// </summary>
        /// <param name="orderId">The orderId to get</param>
        /// <returns>Response codes, returns OK with the order if successful</returns>
        [HttpGet("get/orders/orderId={orderId}", Name = "Get an invoiceItem that contains this orderId")]
        public async Task<IActionResult> GetInvoiceOrderByOrderid([FromRoute] int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.InvoiceOrders.Any() || !_context.Invoices.Any())
            {
                return NotFound();
            }
            if (!_context.InvoiceOrders.Any(b => b.orderId == orderId))
            {
                return NotFound();
            }
            var order = await _context.InvoiceOrders.Where(b => b.invoiceId == orderId).ToListAsync();
            return Ok(order.FirstOrDefault());
        }

        /// <summary>
        /// Adds an invoice and invoiceOrders based on parameters
        /// </summary>
        /// <param name="buyerId">The ID of the buyer</param>
        /// <param name="orderIds">The IDs of the orders to add as a List<int></param>
        /// <param name="costs">The costs of the relevant items in orderIds as List<double></param>
        /// <returns></returns>
        [HttpPost("add/buyerId={buyerId}&cost={cost}&orderIds={orderIds}", Name = "Add an invoice")]
        public async Task<IActionResult> AddInvoice([FromRoute] string buyerId, [FromRoute] List<int> orderIds , [FromRoute] List<double> costs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var itemToAdd = new InvoiceItem {customerId = buyerId};
                await _context.Invoices.AddAsync(itemToAdd);
                await _context.SaveChangesAsync();

                var invoiceId = itemToAdd.id;
                int index = 0;
                while (index < orderIds.Count())
                {
                    await _context.InvoiceOrders.AddAsync(new InvoiceOrder { invoiceId = invoiceId , orderId = orderIds[index] , cost = costs[index]});
                    index++;
                }
                await _context.SaveChangesAsync();

                return Ok(itemToAdd);
            }
        }
    }
}
