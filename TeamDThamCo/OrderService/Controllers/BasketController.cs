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

        // GET: api/Basket/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasketItem([FromRoute] string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var basketItem = await _context.Baskets.SingleOrDefaultAsync(m => m.buyerId == userid);

            if (basketItem == null)
            {
                return NotFound();
            }

            return Ok(basketItem);
        }
        
        // POST: api/Basket
        [HttpPost]
        public async Task<IActionResult> PostBasketItem([FromBody] BasketItem basketItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Baskets.Add(basketItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBasketItem", new { id = basketItem.id }, basketItem);
        }

        // DELETE: api/Basket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasketItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var basketItem = await _context.Baskets.SingleOrDefaultAsync(m => m.id == id);
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