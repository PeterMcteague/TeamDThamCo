using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasketService.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BasketService.Controllers
{
    [Produces("application/json")]
    [Route("api/Basket")]
    public class BasketController : Controller
    {
        private readonly BasketContext _context;

        public BasketController(BasketContext context)
        {
            _context = context;
        }

        [HttpGet("", Name = "Get all baskets")]
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
            var orders = await _context.Baskets.ToListAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Gets the basket of a customer.
        /// </summary>
        /// <param name="User ID"></param>  
        /// <response code="200">Returns the basket</response>
        /// <response code="400">If not any basket items by userid</response>  
        /// <response code="404">If parameters are invalid</response>
        [HttpGet("{userid}", Name = "Get basket by buyer ID")]
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
            if (!_context.Baskets.Any(b => b.buyerId == userid))
            {
                return NotFound();
            }
            var orders = await _context.Baskets.Where(b => b.buyerId == userid).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{userid}&{productid}", Name = "Get basket item by buyer ID and productid")]
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
            var orders = await _context.Baskets.Where(b => b.buyerId == userid && b.productId == productid).ToListAsync();
            return Ok(orders);
        }

        [HttpPost("add/userId={userId}&productId={productId}&quantity={quantity}", Name = "Add an item to a customers basket")]
        public async Task<IActionResult> AddItemToBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (_context.Baskets.Any(b => b.buyerId == userId && b.productId == productId))
                {
                    var basketItem = _context.Baskets.AsNoTracking().FirstOrDefault(b => b.buyerId == userId && b.productId == productId);
                    basketItem.quantity = basketItem.quantity + quantity;
                    _context.Baskets.Update(basketItem);
                    await _context.SaveChangesAsync();
                    return Ok(basketItem);
                }
                else
                {
                    var itemToAdd = new BasketItem {buyerId = userId, productId = productId, quantity = quantity };
                    await _context.Baskets.AddAsync(itemToAdd);
                    await _context.SaveChangesAsync();
                    return Ok(itemToAdd);
                }
            }
        }
        
        [HttpPut("update/userId={userId}&productId={productId}&quantity={quantity}", Name = "Update an items quantity a customers basket")]
        public async Task<IActionResult> UpdateItemInBasket([FromRoute] string userId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Baskets.Any() || !(_context.Baskets.Any(b => b.buyerId == userId && b.productId == productId)))
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
        
        [HttpDelete("delete/userId={userId}&productId={productId}")]
        public async Task<IActionResult> DeleteBasketItem([FromRoute] string userId, [FromRoute] int productId)
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
