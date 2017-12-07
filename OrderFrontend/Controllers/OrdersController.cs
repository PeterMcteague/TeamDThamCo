using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe;
using System.Net.Http;
using OrderFrontend.Models;
using OrderFrontend.Clients;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderFrontend.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly OrderClient orderClient;
        private readonly BasketClient basketClient;
        private readonly PaymentClient paymentClient;

        public OrdersController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.orderClient = new OrderClient(configuration);
            this.basketClient = new BasketClient(configuration);
            this.paymentClient = new PaymentClient(configuration);
        }

        // GET: /<controller>/Checkout
        public async Task<IActionResult> Checkout()
        {
            CheckoutDTO model = new CheckoutDTO();

            model.stripeKey = configuration.GetSection("Keys").GetValue<String>("StripePub");
            model.continueUrl = configuration.GetConnectionString("ProductService");

#if DEBUG
            // Test data because we don't know where to get it from
            model.buyerId = "test-id-plz-ignore";
            model.canPurchase = true;
            model.buyerAddress = "A house, A town, A city, APO STC";
#else
            // Not sure where we're getting this from tbh , either customer deets service or from token
            model.buyerId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();

            var userClient = new CustomerClient(configuration);
            var userResponse = await userClient.GetUser(model.buyerId);
            if (userResponse.IsSuccessStatusCode)
            {
                model.buyerId = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).id;
                model.buyerAddress = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).address;
                model.canPurchase = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).canBuy;
            }
#endif
            var basket = await basketClient.GetBasketAsync(model.buyerId);
            if (basket == null)
            {
                return View(model);
            }

            var cost = 0.00m;
            var orderItems = new List<OrderItem>();
            foreach (BasketItem b in basket)
            {
                orderItems.Add(new OrderItem() { active = true, cost = b.cost, itemName = b.name, orderId = 0, productId = b.productId, quantity = b.quantity });
                cost += (b.cost * b.quantity);
            }
            model.items = orderItems;
            model.totalCost = cost;

            return View(model);
        }

        // For already placed orders
        // GET: /<controller>/ExistingItemPayment
        public async Task<IActionResult> ExistingItemPayment(int OrderId)
        {
#if DEBUG
            var buyerId = "test-id-plz-ignore";
            var canPurchase = true;
            var buyerAddress = "A house, A town, A city, APO STC";
#else
            // Not sure where we're getting this from tbh , either customer deets service or from token
            var buyerId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();

            var userClient = new CustomerClient(configuration);
            var userResponse = await userClient.GetUser(model.buyerId);
            if (userResponse.IsSuccessStatusCode)
            {
                var buyerAddress = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).address;
                var canPurchase = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).canBuy;
            }
#endif
            var orders = orderClient.GetOrdersAsync().Result as List<Order>;
            var orderItems = orderClient.GetOrderItemsAsync(OrderId).Result as List<OrderItem>;
            if (canPurchase && !orders.Where(b => b.id == OrderId).FirstOrDefault().paid)
            {
                return View("ExistingPayment",new CheckoutExistingDTO()
                {
                    buyerAddress = buyerAddress,
                    buyerId = buyerId,
                    canPurchase = canPurchase,
                    continueUrl = configuration.GetConnectionString("ProductService"),
                    stripeKey = configuration.GetSection("Keys").GetValue<String>("StripePub"),
                    items = orderItems,
                    totalCost = orderItems.Sum(b => (b.cost * b.quantity)),
                    OrderId = OrderId
                });
            }
            else
            {
                return Redirect(configuration.GetConnectionString("ProductService"));
            }
        }

        // POST: /<controller>/Payment
        public async Task<IActionResult> PostExistingPayment(StripeToken token, CheckoutExistingDTO dto)
        {
            // Post payment
            var response = paymentClient.PostPayment(dto.totalCost, token.Id);

            if (response.Result)
            {
                await orderClient.UpdateOrderPaid(dto.OrderId, true);

                return Redirect(configuration.GetConnectionString("ProductService"));
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: /<controller>/Payment
        public async Task<IActionResult> Payment(CheckoutDTO dto)
        {
            if (dto.canPurchase)
            {
                return View(dto);
            }
            else
            {
                var response = await orderClient.AddOrder(dto.buyerId, dto.buyerAddress) as Order;
                var orderId = (JsonConvert.DeserializeObject(response.ToString()) as Order).id;
                foreach (OrderItem i in dto.items)
                {
                    await orderClient.AddOrderItem(orderId, i.productId, i.itemName, i.quantity, i.cost);
                }
                var basketResponse = await basketClient.DeleteBasket(dto.buyerId);
                return Redirect(configuration.GetConnectionString("ProductService"));
            }
        }

        // POST: /<controller>/Payment
        public async Task<IActionResult> PostPayment(StripeToken token , CheckoutDTO dto)
        {
            // Post payment
            var response = paymentClient.PostPayment(dto.totalCost,token.Id);

            if (response.IsCompletedSuccessfully)
            {
                var response2 = await orderClient.AddOrder(dto.buyerId, dto.buyerAddress) as Order;
                var orderId = (JsonConvert.DeserializeObject(response2.ToString()) as Order).id;
                foreach (OrderItem i in dto.items)
                {
                    await orderClient.AddOrderItem(orderId, i.productId, i.itemName, i.quantity, i.cost);
                }
                await orderClient.UpdateOrderPaid(orderId,true);
                
                var basketResponse = await basketClient.DeleteBasket(dto.buyerId) as HttpResponseMessage;
                if (basketResponse.IsSuccessStatusCode)
                {
                    return Redirect(configuration.GetConnectionString("ProductService"));
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: /<controller>/History
        public async Task<IActionResult> History()
        {
            String userId;
#if DEBUG
            userId = "test-id-plz-ignore";
            var canPurchase = true;
#else
            // Not sure where we're getting this from tbh , either customer deets service or from token
            userId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();
            
            var userClient = new CustomerClient(configuration);
            var userResponse = await userClient.GetUser(model.buyerId);
            if (userResponse.IsSuccessStatusCode)
            {
                var canPurchase = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).canBuy;
            }
#endif
            var orders = await orderClient.GetOrdersAsync(userId) as List<Order>;
            var orderItems = await orderClient.GetOrderItemsAsync(userId) as List<OrderItem>;

            var dto = new HistoryDTO();
            if (orders == null)
            {
                return View(null);
            }
            var list = new List<OrderAndItemDTO>();
            decimal total = 0.00m;
            foreach (Order o in orders)
            {
                var items = orderItems.Where(b => b.orderId == o.id).ToList();
                var sum = orderItems.Where(b => b.orderId == o.id).Sum(x => x.cost * x.quantity);
                total += sum;
                list.Add(new OrderAndItemDTO { Order = o, Items = items });
            }
            dto.total = total;
            dto.orderAndItems = list;
            dto.canPurchase = canPurchase;
            return View(dto);
        }

        public IActionResult Error(String message)
        {
            return View(message);
        }
    }
}
