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
    [Route("api/Basket")]
    public class BasketController : Controller
    {
        private readonly OrderServiceContext _context;

        public BasketController(OrderServiceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the basket of a customer.
        /// </summary>
        /// <param name="User ID"></param>  
        /// <response code="200">Returns the basket</response>
        /// <response code="400">If not any basket items by userid</response>  
        /// <response code="404">If parameters are invalid</response>
        // GET: api/Basket/5
        [HttpGet("/{userid}", Name = "Get basket by buyer ID")]
        public async Task<IActionResult> GetBasket([FromRoute] string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any())
            {
                return NotFound();
            }
            if (!_context.Baskets.Where(b => b.buyerId == userid).Any())
            {
                return NoContent();
            }

            var orders = _context.Orders.Where(b => b.buyerId == userid);
            return Ok(orders);
        }
        
        // Put: api/Basket/add/userId=5&productId=5&quantity=5
        [HttpPut("/add/userId={userId}&productId={productId}&quantity={quantity}", Name = "Add an item to a customers basket")]
        public async Task<IActionResult> AddItemToBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any())
            {
                return NotFound();
            }
            else
            {
                if (_context.Baskets.Where(b => b.buyerId == userId && b.productId == productId).Any())
                {
                    var basketItem = _context.Baskets.FirstOrDefault(b => b.buyerId == userId && b.productId == productId);
                    basketItem.quantity = basketItem.quantity + quantity;
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

        // Put: api/Basket/update/userId=5&productId=5&quantity=5
        [HttpPut("/update/userId={userId}&productId={productId}&quantity={quantity}", Name = "Update an items quantity a customers basket")]
        public async Task<IActionResult> UpdateItemInBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any() || !_context.Baskets.Where(b => b.buyerId == userId && b.productId == productId).Any())
            {
                return NotFound();
            }
            else
            {
                var basketItem = _context.Baskets.FirstOrDefault(b => b.buyerId == userId && b.productId == productId);
                basketItem.quantity = quantity;
                await _context.SaveChangesAsync();
                return Ok(basketItem);
            }
        }

        // DELETE: api/Basket/delete/userId=5&productId=5
        [HttpDelete("/delete/userId={userId}&productId={productId}")]
        public async Task<IActionResult> DeleteBasketItem([FromRoute] string userId , [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any())
            {
                return NotFound();
            }

            var basketItem = await _context.Baskets.SingleOrDefaultAsync(m => m.productId == productId && m.buyerId == userId);

            if (basketItem == null)
            {
                return NotFound();
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