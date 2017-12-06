using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// A HttpClient for sending invoices to the messaging service

namespace OrderService.Clients
{
    public class MessageSender : HttpClient
    {
        // Database context
        private readonly OrderServiceContext _context;

        /// <summary>
        /// Initializes the client for sending HTTP Requests
        /// </summary>
        /// <param name="context">The order database context</param>
        public MessageSender(OrderServiceContext context)
        {
            _context = context;
            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("MessagingService"));
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Gets items in an order for invoicing
        /// </summary>
        /// <param name="orderId">The ID of the order to return items for</param>
        /// <returns>A list of order items containing all order items in an order</returns>
        private List<OrderItem> getItems(int orderId)
        {
            return _context.OrderItems.Where(b => b.active == true && b.orderId == orderId).ToList();
        }

        /// <summary>
        /// Calls SendMessage for a series of orders that are to be invoiced
        /// </summary>
        /// <param name="orders">The orders to invoice</param>
        /// <returns>Returns the API response</returns>
        public async Task<HttpResponseMessage> SendOrderInvoice(List<Order> orders)
        {
            String message = "";
            String buyerId = orders.FirstOrDefault().buyerId;
            foreach (Order o in orders)
            {
                message += "Order placed " + o.orderDate.Day.ToString() + "/" + o.orderDate.Month.ToString() + "/" + o.orderDate.Year.ToString() + "\n";
                message += "Dispatch to " + o.address;
                decimal total = 0.00m;
                List<OrderItem> items = getItems(o.id);
                String productsString = "";
                foreach (OrderItem i in items)
                {
                    total = +i.cost * i.quantity;
                    productsString += i.itemName + " * " + i.quantity + "   £" + i.cost + "\n";
                }
                message += "Total " + total + "\n";
                message += productsString + "\n\n";
            }
            return await SendMessage(buyerId,message);
        }

        /// <summary>
        /// Sends invoice messages to buyers
        /// </summary>
        /// <param name="buyerId">The buyer to send the messages to</param>
        /// <param name="message">The message to send</param>
        /// <returns>Returns the HTTP response from the messaging service</returns>
        public async Task<HttpResponseMessage> SendMessage(String buyerId , String message)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(JsonConvert.SerializeObject(message)), "message");
                formData.Add(new StringContent(JsonConvert.SerializeObject(buyerId)), "buyerId");

                //TODO Check this is the right URL with service creator
                HttpResponseMessage response = await this.PostAsync("/post/message/buyerId={buyerId}&message={message}", formData);
                response.EnsureSuccessStatusCode();

                return response;
            }
           
        }
    }
}
