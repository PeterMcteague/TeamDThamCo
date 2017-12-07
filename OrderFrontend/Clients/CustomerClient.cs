using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrderFrontend.Clients
{
    public class CustomerClient : HttpClient
    {
        private readonly IConfiguration _configuration;

        public CustomerClient(IConfiguration configuration)
        {
            this._configuration = configuration;
            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("CustomerService"));
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> GetUser(String id)
        {
            try
            {
                HttpResponseMessage response = await this.GetAsync("/api/user/id=" + id);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}
