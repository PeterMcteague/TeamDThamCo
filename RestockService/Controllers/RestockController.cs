using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using RestockService.Models;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace RestockService.Controllers
{
    [Route("api/[controller]")]
    public class RestockController : Controller
    {
        private readonly RestockContext _context;

        public RestockController(RestockContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This returns all the products from all the suppliers
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("get", Name = "List all products")]
        [Authorize]
        public async Task<IActionResult> getSupplierProducts()
        {
            if (!_context.Suppliers.Any())
            {
                return NoContent();
            }
            var products = new List<Product>();
            foreach (Supplier s in _context.Suppliers)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(s.GetUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.GetAsync("").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync();
                        jsonString.Wait();
                        var modelObject = JsonConvert.DeserializeObject<List<Product>>(jsonString.Result);
                        foreach (Product p in modelObject)
                        {
                            p.SupplierId = s.Id;
                            p.SupplierProductId = p.Id;
                        }
                        products.AddRange(modelObject);
                    }
                }
            }
            return Ok(products);
        }

        /// <summary>
        /// This returns a specific product by its id
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpGet("get/id={id}supplierId={supplierId}", Name = "return product by id")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [Authorize]
        public IActionResult GetSupplierProductById(int Id, int supplierId)
        {
            using (var client = new HttpClient())
            {
                if (Id <= 0)
                {
                    return BadRequest();
                }
                Supplier s = _context.Suppliers.First(b => b.Id == supplierId);
                client.BaseAddress = new Uri(s.GetUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync();
                    jsonString.Wait();
                    var item = JsonConvert.DeserializeObject<Product>(jsonString.Result);
                    if (item != null)
                    {
                        return Ok(item);
                    }
                }
            }
            return NotFound();
        }

        /// <summary>
        /// Posts to API to purchase Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>

        [HttpPost("post/Product={Product}&Card={Card}", Name = "buys a product")]
        [Authorize]
        public async Task<Uri> PurchaseProduct(Product product, Card card)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_context.Suppliers.Where(b => b.Id == product.SupplierId).FirstOrDefault().OrderUri);
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(JsonConvert.SerializeObject(product)), "product");
                formData.Add(new StringContent(JsonConvert.SerializeObject(card)), "card");

                HttpResponseMessage response = await client.PostAsync("api/Order", formData);
                response.EnsureSuccessStatusCode();

                // return URI of the created resource.
                return response.Headers.Location;
            }
        }

        /// <summary>
        /// This removes a card by cardnumber
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpDelete("delete/CardNumber={CardNumber}", Name = "removes a card")]
        [Authorize]
        public async Task<IActionResult> DeleteCard(string CardNumber)
        {
            var log = _context.Cards.SingleOrDefault(x => x.CardNumber == CardNumber);

            if (log == null)
            {
                return NotFound();
            }

            _context.Cards.Remove(log);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// This adds a new card
        /// </summary>
        /// <returns>returns a HTTP response if successful</returns>
        [HttpPost("post/AccountName={AccountName}&CardNumber={CardNumber}", Name = "adds a new card")]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromBody]Card card)
        {
            var item = new Card
            {
                AccountName = card.AccountName,
                CardNumber = card.CardNumber,
                
            };
            _context.Cards.Add(item);

            await _context.SaveChangesAsync();

            return Ok(item);
        }
    }
}
