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
            var orders = await _context.Invoices.ToListAsync();
            return Ok(orders);
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
            var orders = await _context.Invoices.Take(quantity).ToListAsync();
            return Ok(orders);
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
            if (!_context.Invoices.Any())
            {
                return NotFound();
            }
            var orders = await _context.Invoices.Where(b => b.customerId == customerId).ToListAsync();
            return Ok(orders);
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
            if (!_context.Invoices.Any())
            {
                return NotFound();
            }
            var orders = await _context.Invoices.Where(b => b.customerId == customerId).Take(quantity).ToListAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Adds a new invoice.
        /// </summary>
        /// <param name="buyerId">The id of the buyer</param>
        /// <param name="address">Their address</param>
        /// <returns>Returns the added item.</returns>
        [HttpPost("add/buyerId={buyerId}&cost={cost}&orderIds={orderIds}", Name = "Add an invoice")]
        public async Task<IActionResult> AddInvoice([FromRoute] string buyerId, [FromRoute] double cost, [FromRoute] List<int> orderIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var itemToAdd = new InvoiceItem {customerId = buyerId , cost = cost, orderIds = orderIds };
                await _context.Invoices.AddAsync(itemToAdd);
                await _context.SaveChangesAsync();
                return Ok(itemToAdd);
            }
        }
    }
}
