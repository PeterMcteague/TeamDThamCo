using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasketService.Models;
using Microsoft.EntityFrameworkCore;
using BasketService.Data;
using Microsoft.AspNetCore.Authorization;

// A controller for the basket service API

namespace BasketService.Controllers
{
    [Produces("application/json")]
    [Route("api/Basket/")]
    public class BasketAPIController : Controller
    {
        private readonly BasketContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The basket database context</param>
        public BasketAPIController(BasketContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all baskets
        /// </summary>
        /// <returns>Returns all baskets</returns>
        /// <response code="200">Returns the basket</response>
        /// <response code="400">If the parameters sent are invalid</response>
        /// <response code="404">If not any baskets</response>  
        [Authorize]
        [HttpGet("get/", Name = "Get all baskets")]
        public async Task<IActionResult> GetBaskets()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any())
            {
                return NotFound();
            }
            var baskets = await _context.Baskets.ToListAsync();
            return Ok(baskets);
        }

        /// <summary>
        /// Gets the basket of a customer.
        /// </summary>
        /// <param name="userid">The userid to get the basket of</param>  
        /// <response code="200">Returns the basket</response>
        /// <response code="400">If the parameters sent are invalid</response>  
        /// <response code="404">If there aren't any baskets owned by that userid</response>
        [Authorize]
        [HttpGet("get/{userid}", Name = "Get baskets by buyer ID")]
        public async Task<IActionResult> GetBaskets([FromRoute] string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any(b => b.buyerId == userid))
            {
                return NotFound("No baskets found");
            }
            var baskets = await _context.Baskets.Where(b => b.buyerId == userid).ToListAsync();
            return Ok(baskets);
        }

        /// <summary>
        /// Gets a range of basket items , useful for pages
        /// </summary>
        /// <param name="userid">The userId that the basket is owned by</param>
        /// <param name="start">The start of the range</param>
        /// <param name="end">The end of the range</param>
        /// <response code="200">Returns the baskets</response>
        /// <response code="400">If the parameters sent are invalid</response>  
        /// <response code="404">If there aren't baskets owned by that userId</response>
        [Authorize]
        [HttpGet("get/{userid}&range={start}-{end}", Name = "Get basket by buyer ID in range start-end")]
        public async Task<IActionResult> GetBaskets([FromRoute] string userid , [FromRoute] int start, [FromRoute] int end)
        {
            if (!ModelState.IsValid || start >= end)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any(b => b.buyerId == userid))
            {
                return NotFound("No baskets found");
            }
            var baskets = await _context.Baskets.Where(b => b.buyerId == userid).Skip(start).Take(end - start).ToListAsync();
            return Ok(baskets);
        }

        /// <summary>
        /// Get an item from a basket by productid
        /// </summary>
        /// <param name="userid">The userid to get by</param>
        /// <param name="productid">The productid to get by</param>
        /// <response code="200">Returns the basket item</response>
        /// <response code="400">If the parameters sent are invalid</response>  
        /// <response code="404">If there aren't baskets with the parameters</response>
        [Authorize]
        [HttpGet("get/{userid}&{productid}", Name = "Get basket item by buyer ID and productid")]
        public async Task<IActionResult> GetBasketItem([FromRoute] string userid, [FromRoute] int productid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any())
            {
                return NotFound();
            }
            if (!_context.Baskets.Any(b => b.buyerId == userid && b.productId == productid))
            {
                return NotFound();
            }
            var basketItems = await _context.Baskets.Where(b => b.buyerId == userid && b.productId == productid).ToListAsync();
            return Ok(basketItems);
        }

        /// <summary>
        /// Adds a basket record. If the basket item already exists it adds to quantity.
        /// </summary>
        /// <param name="userId">The userId to add the item to.</param>  
        /// <param name="productId">The productId to add to the basket.</param>  
        /// <param name="quantity">The amount to add.</param>  
        /// <response code="200">OK. Returns the item added.</response>
        /// <response code="400">If parameters invalid.</response>  
        [Authorize]
        [HttpPost("add/userId={userId}&productId={productId}&quantity={quantity}&productName={name}&cost={cost}", Name = "Add an item to a customers basket")]
        public async Task<IActionResult> AddItemToBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity, [FromRoute] string name , [FromRoute] decimal cost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (_context.Baskets.Where(b => b.buyerId == userId && b.productId == productId).Any())
                {
                    var basketItem = _context.Baskets.FirstOrDefault(b => b.buyerId == userId && b.productId == productId);
                    basketItem.quantity = basketItem.quantity + quantity;
                    _context.Update(basketItem);
                    await _context.SaveChangesAsync();
                    return Ok(basketItem);
                }
                else
                {
                    var itemToAdd = new BasketItem { buyerId = userId, productId = productId, quantity = quantity, name = name, cost = cost };
                    _context.Baskets.Add(itemToAdd);
                    await _context.SaveChangesAsync();
                    return Ok(itemToAdd);
                }
            }
        }

        /// <summary>
        /// Updates a basket record.
        /// </summary>
        /// <param name="userId">The userId to add the item to.</param>  
        /// <param name="productId">The productId to add to the basket.</param>  
        /// <param name="quantity">The updated amount.</param>  
        /// <response code="200">OK. Returns the item added.</response>
        /// <response code="400">If parameters invalid.</response>
        /// <response code="404">If basket item to update not found.</response>
        [Authorize]
        [HttpPut("update/userId={userId}&productId={productId}&quantity={quantity}", Name = "Update an items quantity a customers basket")]
        public async Task<IActionResult> UpdateItemInBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Where(b => b.buyerId == userId && b.productId == productId).Any())
            {
                return NotFound("No item found with those arguments");
            }
            if (quantity == 0)
            {
                return BadRequest("Quantity is 0. Please use the delete method for this.");
            }
            else
            {
                var basketItem = _context.Baskets.FirstOrDefault(b => b.buyerId == userId && b.productId == productId);
                basketItem.quantity = quantity;
                _context.Update(basketItem);
                await _context.SaveChangesAsync();
                return Ok(basketItem);
            }
        }

        /// <summary>
        /// Deletes a basket item.
        /// </summary>
        /// <param name="userId">The userId to add the item to.</param>  
        /// <param name="productId">The productId to add to the basket.</param>  
        /// <response code="200">OK. Returns the item added.</response>
        /// <response code="400">If parameters invalid.</response>
        /// <response code="404">If item to delete not found.</response>
        [Authorize]
        [HttpDelete("delete/userId={userId}&productId={productId}")]
        public async Task<IActionResult> DeleteBasketItem([FromRoute] string userId , [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any())
            {
                return NotFound("No baskets found");
            }

            var basketItem = await _context.Baskets.SingleOrDefaultAsync(m => m.productId == productId && m.buyerId == userId);

            if (basketItem == null)
            {
                return NotFound("No basket items found with those arguments");
            }

            _context.Baskets.Remove(basketItem);
            await _context.SaveChangesAsync();

            return Ok(basketItem);
        }

        private bool BasketItemExists(int id)
        {
            return _context.Baskets.Any(e => e.id == id);
        }
    }
}
