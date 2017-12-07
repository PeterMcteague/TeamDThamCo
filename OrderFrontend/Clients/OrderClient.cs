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
    public class OrderClient : HttpClient
    {
        private class paidDto
        {
            public bool paid;
            public int id;
        }

        private class addDto
        {
            public String buyerId;
            public String address;
        }

        private class addItemDto
        {
            public int orderId;
            public int productId;
            public string name;
            public int quantity;
            public decimal cost;
        }

        private readonly IConfiguration _configuration;

        public OrderClient(IConfiguration configuration)
        {
            this._configuration = configuration;

            // Get auth token
            var bearer = new AuthClient().getOrderBearer();

            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("OrderService") + "/api/Orders/");
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Add("authorization", bearer);
        }

        public async Task<Order> AddOrder(String buyerId , String address)
        {
            try
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(new addDto {buyerId = buyerId,address=address }));
                var response = await this.PostAsync("Add/buyerId={buyerId}&address={address}",stringContent);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Order>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<OrderItem> AddOrderItem(int orderId, int productId, string name, int quantity, decimal cost)
        {
            try
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(new addItemDto { orderId=orderId,productId=productId,name=name,quantity=quantity,cost=cost}));
                var response = await this.PostAsync("Products/Add/orderId={orderId}&productId={productId}&name={name}&quantity={quantity}&cost={cost}", stringContent);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OrderItem>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            try
            {
                var response = await this.GetAsync("get/");
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Order>>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Order>> GetOrdersAsync(string buyerId)
        {
            try
            {
                var response = await this.GetAsync("get/" + buyerId);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Order>>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            try
            {
                var response = await this.GetAsync("Products/Get/orderid=" + orderId);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<OrderItem>>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(string buyerId)
        {
            try
            {
                var response = await this.GetAsync("Products/Get/buyerid=" + buyerId);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<OrderItem>>(stringResult);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Order> UpdateOrderPaid(int orderId , bool paid)
        {
            try
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(new paidDto() { id = orderId,paid=paid }));
                var response = await this.PutAsync("Update/id={id}&paid={paid}",stringContent);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Order>(stringResult);
            }
            catch
            {
                return null;
            }
        }
    }
}
