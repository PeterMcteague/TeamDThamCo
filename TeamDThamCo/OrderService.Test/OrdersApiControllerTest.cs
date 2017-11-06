using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OrderService;
using OrderService.Models;

namespace OrderService.Test
{
    public class OrdersApiControllerTest
    {
        private readonly Controllers.OrdersAPIController _orderController;

        public OrdersApiControllerTest()
        {
            _orderController = new Controllers.OrdersAPIController(null);
        }

        //GET /api/Orders
        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            var result = await _orderController.GetOrder() as List<Order>;
            Assert.Equal(getTestOrders().Count, result.Count);
        }

        //GET /api/Orders/test-id-plz-ignore
        [Fact]
        public async Task GetOrdersIdAsync_ShouldReturnIdsOrders()
        {
            var result = await _orderController.GetOrder("test-id-plz-ignore") as List<Order>;
            Assert.Equal(getTestOrders("test-id-plz-ignore").Count, result.Count);
        }

        //GET /api/Orders/invalid-id
        [Fact]
        public async Task GetOrdersIdAsyncWrong_ShouldReturnNoOrders()
        {
            var result = await _orderController.GetOrder("fake-id") as List<Order>;
            Assert.Equal(0, result.Count);
        }

        //GET /api/Orders/Products
        [Fact]
        public async Task GetOrderProductsAsync_ShouldReturnAllProductsOrdered()
        {
            var result = await _orderController.GetProductsOrdered() as List<Order>;
            Assert.Equal(getTestProducts().Count, result.Count);
        }

        //GET /api/Orders/Products/{id}
        [Fact]
        public async Task GetOrderProductsAsyncById_ShouldReturnAllProductsOrderedById()
        {
            var result = await _orderController.GetProductsInOrder(1) as List<Order>;
            Assert.Equal(getTestProducts(1).Count, result.Count);
        }

        //GET /api/Orders/Products/invalid-id
        [Fact]
        public async Task GetOrderProductsAsyncByIdWrong_ShouldReturnNoProducts()
        {
            var result = await _orderController.GetProductsInOrder(-267) as List<Order>;
            Assert.Equal(0 , result.Count);
        }

        private List<Order> getTestOrders(string id = "")
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order { id = 1, invoiced = true, dispatched = true, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01") });
            if (id != "")
            {
                return orders.Where(b => b.buyerId == id).ToList();
            }
            else
            {
                return orders;
            }
        }

        private List<OrderItem> getTestProducts(int id = 0)
        {
            List<OrderItem> testProducts = new List<OrderItem>();
            testProducts.Add(new OrderItem { orderId = 1, name = "Premium Jelly Beans", cost = 2.00, quantity = 5 });
            return testProducts.Where(b => b.orderId == id).ToList();
        }
    }
}
