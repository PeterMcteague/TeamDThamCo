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
    public class PaymentClient : HttpClient
    {
        private readonly IConfiguration _configuration;

        private class paymentDTO
        {
            public Decimal totalGbp;
            public string stripeToken;
        }

        public PaymentClient(IConfiguration configuration)
        {
            this._configuration = configuration;

            // Get auth token
            var bearer = new AuthClient().getOrderBearer();

            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("OrderService") + "/api/Payment");
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Add("authorization", bearer);
        }

        public async Task<Boolean> PostPayment(Decimal totalGbp, string stripeTokenId)
        {
            try
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(new paymentDTO() { totalGbp = totalGbp,stripeToken=stripeTokenId}));
                var response = await this.PostAsync("/MakePayment",stringContent);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
