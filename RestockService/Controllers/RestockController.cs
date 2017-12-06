using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using RestockService.Models;
using Newtonsoft.Json;

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
                    client.BaseAddress = s.GetUri;
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


        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
