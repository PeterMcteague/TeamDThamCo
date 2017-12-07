using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using System.Net;

namespace ProductService.Controllers
{
    [Produces("application/json")]
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private readonly ProductContext _context;

        public ProductController(ProductContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This returns a specific product by its id
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("api/Product/{id:int}", Name = "return product by id")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetItemById(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }

            var item = await _context.Products.SingleOrDefaultAsync(ci => ci.Id == Id);
            if (item != null)
            {
                return Ok(item);
            }

            return NotFound();
        }

        /// <summary>
        /// This lists all the products
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("get",Name ="List all products")]
        public async Task<IActionResult> getProducts()
        {
            if (!_context.Products.Any())
            {
                return NotFound();
            }
            else
            {
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
        }

        /// <summary>
        /// This lists all the products containing the search query
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("get/name={name}", Name = "search by name")]
        public async Task<IActionResult> searchProducts([FromRoute] string name)
        {
            if (!_context.Products.Any())
            {
                return NotFound();
            }
            else
            {
                var products = await _context.Products.Where(x => x.Name.Contains(name)).ToListAsync();
                return Ok(products);
            }
        }

        /// <summary>
        /// This lists x of the products per page
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("get/page={page}/pageSize={pageSize}", Name = "Lists all products at 10 products per page")]
        public async Task<IActionResult> Items([FromQuery]int page, [FromQuery]int pageSize)
        {
            var itemsOnPage = await _context.Products
                .OrderBy(c => c.Name)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(itemsOnPage);
        }

        /// <summary>
        /// This lists x of the price change logs per page
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("get/page={page}/pageSize={pageSize}", Name = "Lists all products at 10 logs per page")]
        public async Task<IActionResult> PriceHistory([FromQuery]int page, [FromQuery]int pageSize)
        {
            var itemsOnPage = await _context.PriceLogs
                .OrderBy(c => c.UpdateDate)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(itemsOnPage);
        }


        /// <summary>
        /// This updates a products stock
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpPut("put/Id={Id}&StockNumber={StockNumber}", Name = "Updates a products stock")]

        public async Task<IActionResult> UpdateProductStock([FromBody]Product productToUpdate)
        {
            var catalogItem = await _context.Products.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} has not been located." });
            }

            var oldStock = catalogItem.StockNumber;
            var changeStock = oldStock != productToUpdate.StockNumber;

            catalogItem = productToUpdate;
            _context.Products.Update(catalogItem);

            await _context.SaveChangesAsync();

            return Ok(productToUpdate);

        }

        /// <summary>
        /// This updates a products price
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpPut("put/Id={Id}&Price={Price}", Name = "Updates a products price")]

        public async Task<IActionResult> UpdateProductPrice([FromBody]int productId , [FromBody] decimal newPrice)
        {
            var catalogItem = await _context.Products.SingleOrDefaultAsync(i => i.Id == productId);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productId} has not been located." });
            }

            var oldPrice = catalogItem.Price;

            if (oldPrice != newPrice)
            {
                return StatusCode(304); //Not modified
            }
            else
            {
                _context.PriceLogs.Add(new PriceLog {productId = productId,OldPrice=oldPrice,UpdateDate=DateTime.Now });
                catalogItem.Price = newPrice;
                _context.Products.Update(catalogItem);

                await _context.SaveChangesAsync();
                return Ok(catalogItem);
            }
        }



        /// <summary>
        /// This adds a new product
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpPost("post/Name={Name}&BrandId={BrandId}&BrandName={BrandName}&CategoryId={CategoryId}&CategoryName={CategoryName}&Description={Description}&Ean={Ean}&ExpectedRestock={ExpectedRestock}&Id={Id}&InStock={InStock}&StockNumber={StockNumber}&Price={Price}", Name = "adds a product")]
        public async Task<IActionResult> CreateProduct([FromBody]Product product)
        {
            var item = new Product
            {
                BrandId = product.BrandId,
                BrandName = product.BrandName,
                CategoryId = product.CategoryId,
                CategoryName = product.CategoryName,
                Description = product.Description,
                Ean = product.Ean,
                ExpectedRestock = product.ExpectedRestock,
                Id = product.Id,
                InStock = product.InStock,
                StockNumber = product.StockNumber,
                Name = product.Name,
                Price = product.Price
            };
            _context.Products.Add(item);

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        /// <summary>
        /// This removes a product by id
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpDelete("delete/Id={Id}", Name = "removes a product")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _context.Products.SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}