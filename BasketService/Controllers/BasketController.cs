﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasketService.Models;
using Microsoft.EntityFrameworkCore;
using BasketService.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BasketService.Controllers
{
    [Produces("application/json")]
    [Route("api/Basket/")]
    public class BasketController : Controller
    {
        private readonly BasketContext _context;

        public BasketController(BasketContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all baskets
        /// </summary>
        /// <returns>Returns all baskets</returns>
        /// <response code="200">Returns the basket</response>
        /// <response code="400">If not any baskets</response>  
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
        /// <response code="400">If not any basket items by userid</response>  
        /// <response code="404">If parameters are invalid</response>
        [HttpGet("get/{userid}", Name = "Get baskets by buyer ID")]
        public async Task<IActionResult> GetBasket([FromRoute] string userid)
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
        /// Gets X baskets
        /// </summary>
        /// <param name="userid">The userId that owns the baskets</param>
        /// <param name="count">Number of baskets to get</param>
        /// <returns>Result codes</returns>
        [HttpGet("get/{userid}&count={count}", Name = "Get X baskets by buyer ID")]
        public async Task<IActionResult> GetBasket([FromRoute] string userid, [FromRoute] int count)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any(b => b.buyerId == userid))
            {
                return NotFound("No baskets found");
            }
            var baskets = await _context.Baskets.Where(b => b.buyerId == userid).Take(count).ToListAsync();
            return Ok(baskets);
        }

        /// <summary>
        /// Gets a range of items , useful for pages
        /// </summary>
        /// <param name="userid">The userId that the basket is owned by</param>
        /// <param name="start">The start of the range</param>
        /// <param name="end">The end of the range</param>
        /// <returns>Result codes</returns>
        [HttpGet("get/{userid}&range={start}-{end}", Name = "Get basket by buyer ID in range start-end")]
        public async Task<IActionResult> GetBasket([FromRoute] string userid , [FromRoute] int start, [FromRoute] int end)
        {
            if (!ModelState.IsValid || start >= end)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any(b => b.buyerId == userid) || !(_context.Baskets.Count() < end))
            {
                return NotFound("No baskets found");
            }
            var baskets = await _context.Baskets.Where(b => b.buyerId == userid).Skip(start - 1).Take(end - start).ToListAsync();
            return Ok(baskets);
        }

        /// <summary>
        /// Get an item from a basket by productid
        /// </summary>
        /// <param name="userid">The userid to get by</param>
        /// <param name="productid">The productid to get by</param>
        /// <returns></returns>
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
        [HttpPost("add/userId={userId}&productId={productId}&quantity={quantity}", Name = "Add an item to a customers basket")]
        public async Task<IActionResult> AddItemToBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity)
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
                    var itemToAdd = new BasketItem { buyerId = userId, productId = productId, quantity = quantity };
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
