using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Clients;
using Microsoft.Extensions.Configuration;
using OrderService.Data;
using OrderService.Models;
using Newtonsoft.Json;
using Stripe;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly OrderServiceContext _context;

        public OrdersController(IConfiguration configuration, OrderServiceContext context)
        {
            this.configuration = configuration;
            this._context = context;
        }

        // GET: /<controller>/Checkout
        public async Task<IActionResult> Checkout()
        {
            CheckoutDTO model = new CheckoutDTO();

            model.stripeKey = configuration.GetSection("Keys").GetValue<String>("StripePub");
            model.continueUrl = configuration.GetConnectionString("ProductService");

#if DEBUG
            model.buyerId = "test-id-plz-ignore";
            model.canPurchase = true;
            model.buyerAddress = "A house, A town, A city, APO STC";

            List<OrderItem> testBasket = new List<OrderItem>();
            testBasket.Add(new OrderItem { productId = 1, itemName = "Premium Jelly Beans", cost = 0.4m, quantity = 5, active = true });
            testBasket.Add(new OrderItem { productId = 2, itemName = "Netlogo Supercomputer", cost = 2005.99m, quantity = 1, active = true });
            model.items = testBasket;
#else
            // Not sure where we're getting this from tbh , either customer deets service or from token
            model.buyerId = User.Claims.Where(uId => uId.Type == "User_Id").Select(c => c.Value).SingleOrDefault();

            var userClient = new CustomerClient(configuration);
            var userResponse = await userClient.GetUser(model.buyerId);
            if (userResponse.IsSuccessStatusCode)
            {
                model.buyerAddress = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).address;
                model.canPurchase = (JsonConvert.DeserializeObject(userResponse.ToString()) as User).canBuy;
            }

            var basketClient = new OrderClient(configuration);
            var basketResponse = await basketClient.GetBasket(model.buyerId);
            if (basketResponse.IsSuccessStatusCode)
            {
                model.items = (JsonConvert.DeserializeObject(userResponse.ToString()) as List<OrderItem>);
            }
#endif
            var cost = 0.00m;
            foreach (OrderItem i in model.items)
            {
                cost += (i.cost * i.quantity);
            }
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
            if (canPurchase && !_context.Orders.Where(b => b.id == OrderId).FirstOrDefault().paid)
            {
                return View("ExistingPayment",new CheckoutExistingDTO()
                {
                    buyerAddress = buyerAddress,
                    buyerId = buyerId,
                    canPurchase = canPurchase,
                    continueUrl = configuration.GetConnectionString("ProductService"),
                    stripeKey = configuration.GetSection("Keys").GetValue<String>("StripePub"),
                    items = _context.OrderItems.Where(b => b.orderId == OrderId).ToList(),
                    totalCost = _context.OrderItems.Where(b => b.orderId == OrderId).Sum(b => (b.cost * b.quantity)),
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
            var paymentApi = new PaymentAPIController();
            var response = paymentApi.PostPayment(dto.totalCost, token.Id) as HttpResponseMessage;

            if (response.IsSuccessStatusCode)
            {
                var apiControllerInstance = new OrdersAPIController(_context);
                await apiControllerInstance.UpdateOrderPaid(dto.OrderId, true);

                return Redirect(configuration.GetConnectionString("ProductService"));
            }
            else
            {
                return RedirectToAction("Error", response.StatusCode);
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
                var apiControllerInstance = new OrdersAPIController(_context);
                var response = await apiControllerInstance.AddOrder(dto.buyerId, dto.buyerAddress) as Order;
                var orderId = (JsonConvert.DeserializeObject(response.ToString()) as Order).id;
                foreach (OrderItem i in dto.items)
                {
                    await apiControllerInstance.AddOrderItem(orderId, i.productId, i.itemName, i.quantity, i.cost);
                }
                var basketClient = new OrderClient(configuration);
                var basketResponse = await basketClient.DeleteBasket(dto.buyerId);
                return Redirect(configuration.GetConnectionString("ProductService"));
            }
        }

        // POST: /<controller>/Payment
        public async Task<IActionResult> PostPayment(StripeToken token , CheckoutDTO dto)
        {
            // Post payment
            var paymentApi = new PaymentAPIController();
            var response = paymentApi.PostPayment(dto.totalCost,token.Id) as HttpResponseMessage;

            if (response.IsSuccessStatusCode)
            {
                var apiControllerInstance = new OrdersAPIController(_context);
                var response2 = await apiControllerInstance.AddOrder(dto.buyerId, dto.buyerAddress) as Order;
                var orderId = (JsonConvert.DeserializeObject(response2.ToString()) as Order).id;
                foreach (OrderItem i in dto.items)
                {
                    await apiControllerInstance.AddOrderItem(orderId, i.productId, i.itemName, i.quantity, i.cost);
                }
                await apiControllerInstance.UpdateOrderPaid(orderId,true);

                var basketClient = new OrderClient(configuration);
                var basketResponse = await basketClient.DeleteBasket(dto.buyerId) as HttpResponseMessage;
                if (basketResponse.IsSuccessStatusCode)
                {
                    return Redirect(configuration.GetConnectionString("ProductService"));
                }
                else
                {
                    return RedirectToAction("Error", basketResponse.StatusCode);
                }
            }
            else
            {
                return RedirectToAction("Error",response.StatusCode);
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
            var orderApi = new OrdersAPIController(_context);

            var orderResponse = await orderApi.GetOrder(userId) as OkObjectResult;
            var orderItemResponse = await orderApi.GetOrderItemsByBuyerID(userId) as OkObjectResult;

            List<OrderItem> orderItems = new List<OrderItem>();
            List<Order> orders = new List<Order>();

            if (orderResponse.StatusCode == StatusCode(200).StatusCode)
            {
                orders = orderResponse.Value as List<Order>;
            }
            if (orderItemResponse.StatusCode == StatusCode(200).StatusCode)
            {
                orderItems = orderItemResponse.Value as List<OrderItem>;
            }     

            var dto = new HistoryDTO();
            foreach (Order o in orders)
            {
                var items = orderItems.Where(b => b.orderId == o.id);
                dto.orderAndItems.Add(new OrderAndItemDTO(o,items.ToList()));
            }
            dto.canPurchase = canPurchase;

            return View(dto.orderAndItems.OrderBy(o => o.Order.orderDate).ToList());
        }

        public IActionResult Error(String message)
        {
            return View(message);
        }
    }
}
