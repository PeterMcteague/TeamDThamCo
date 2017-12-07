using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrderService.Clients
{
    public class BasketClient : HttpClient
    {
        private readonly IConfiguration _configuration;

        public BasketClient(IConfiguration configuration)
        {
            this._configuration = configuration;
            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("BasketService"));
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> GetBasket(String id)
        {
            HttpResponseMessage response = await this.GetAsync("/api/basket/id=" + id);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> DeleteBasket(String id)
        {
            HttpResponseMessage response = await this.GetAsync("/api/basket/delete/userId=" + id);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
