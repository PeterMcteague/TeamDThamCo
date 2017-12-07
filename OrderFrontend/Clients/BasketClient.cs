using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrderFrontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrderFrontend.Clients
{
    public class BasketClient : HttpClient
    {
        private readonly IConfiguration _configuration;

        public BasketClient(IConfiguration configuration)
        {
            this._configuration = configuration;

            // Get auth token
            var bearer = new AuthClient(configuration).getBasketBearer();

            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("BasketService"));
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Add("authorization", bearer);
        }

        public async Task<List<BasketItem>> GetBasketAsync(String id)
        {
            try
            {
                var response = await this.GetAsync("/api/Basket/get/" + id);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BasketItem>>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> DeleteBasket(String id)
        {
            HttpResponseMessage response = await this.DeleteAsync("/api/basket/delete/userId=" + id);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> GetSingleBasket(String id)
        {
            try
            {
                HttpResponseMessage response = await this.GetAsync("/api/basket/id=" + id);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> DeleteSingleBasket(string userId, int productId)
        {
            HttpResponseMessage response = await this.DeleteAsync("/api/basket/delete/userId=" + userId + "/productId=" + productId);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
