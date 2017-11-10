using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OrderService;
using OrderService.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OrderService.Controllers;
using Newtonsoft.Json;

namespace OrderService.Test
{
    public class OrdersApiControllerTest
    {
        private OrderServiceContext _context;
        private OrdersAPIController _controller;

        public OrdersApiControllerTest()
        {
            // Initialize DbContext in memory
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("ServiceAPITestDB");
            _context = new OrderServiceContext(optionsBuilder.Options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            

            // Seed data
            List<Order> testOrders = new List<Order>();
            List<OrderItem> testProducts = new List<OrderItem>();

            testOrders.Add(new Order {id = 1, invoiced = true, dispatched = true, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01"), active = true });
            testOrders.Add(new Order {id = 2, invoiced = true, dispatched = false, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01"), active = true });

            testProducts.Add(new OrderItem {id = 1, orderId = 1, name = "Premium Jelly Beans", cost = 2.00, quantity = 5, active = true });
            testProducts.Add(new OrderItem {id = 2, orderId = 2, name = "Netlogo Supercomputer", cost = 2005.99, quantity = 1, active = true });

            _context.Orders.AddRange(testOrders);
            _context.OrderItems.AddRange(testProducts);

            foreach (Order s in testOrders)
            {
                if (s.dispatched)
                {
                    _context.Dispatches.Add(new Dispatch { orderId = s.id });
                }
            }
            _context.SaveChanges();

            // Create test subject
            _controller = new OrdersAPIController(_context);
        }

        //GET /api/Orders
        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            var result = await _controller.GetOrder() as ObjectResult;
            var orders = result.Value as IEnumerable<Order>;

            Assert.Equal(getTestOrders().Count(), orders.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GET /api/Orders/test-id-plz-ignore
        [Fact]
        public async Task GetOrdersIdAsync_ShouldReturnIdsOrders()
        {
            var result = await _controller.GetOrder("test-id-plz-ignore") as ObjectResult;
            var orders = result.Value as IEnumerable<Order>;

            Assert.Equal(getTestOrders("test-id-plz-ignore").Count(), orders.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GET /api/Orders/invalid-id
        [Fact]
        public async Task GetOrdersIdAsyncWrong_ShouldReturnNoOrders()
        {
            var result = await _controller.GetOrder("fake-id") as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        //GET /api/Orders/Products
        [Fact]
        public async Task GetOrderProductsAsync_ShouldReturnAllProductsOrdered()
        {
            var result = await _controller.GetProductsOrdered() as ObjectResult;
            var products = result.Value as IEnumerable<OrderItem>; 

            Assert.Equal(getTestProducts().Count(), products.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GET /api/Orders/Products/orderid={orderid}
        [Fact]
        public async Task GetProductsOrderedsAsyncById_ShouldReturnAllProductsByOrderId()
        {
            var result = await _controller.GetProductsOrdered(1) as ObjectResult;
            var products = result.Value as IEnumerable<OrderItem>;

            Assert.Equal(getTestProducts(1).Count(), products.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GET /api/Orders/Products/orderid=invalid-id
        [Fact]
        public async Task GetProductsOrderedAsyncByIdWrong_ShouldReturnNoProducts()
        {
            var result = await _controller.GetProductsOrdered(-267) as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        //GET /api/Orders/Products/buyerid={buyer-id}
        [Fact]
        public async Task GetProductsOrderedAsyncByBuyerId_ShouldReturnTestProducts()
        {
            var result = await _controller.GetProductsOrdered("test-id-plz-ignore") as ObjectResult;
            var products = result.Value as IEnumerable<OrderItem>;

            Assert.Equal(getTestProducts(buyerId:"test-id-plz-ignore").Count(), products.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GET /api/Orders/Products/buyerid=invalid-id
        [Fact]
        public async Task GetProductsOrderedAsyncByInvalidBuyerId_ShouldReturnNoProducts()
        {
            var result = await _controller.GetProductsOrdered("fake-id") as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        //PUT /api/Orders/Delete/orderId={order-id}
        [Fact]
        public async Task DeleteOrder_ShouldReturnWithoutIt()
        {
            var before = await _controller.GetOrder() as ObjectResult;
            var productsBefore = (before.Value as IEnumerable<Order>).Count(); //Have to get the count value before because the delete will happen to this too,  we don't have the data yet!

            var result = await _controller.DeleteOrder(2) as ObjectResult;

            var after = await _controller.GetOrder() as ObjectResult;
            var productsAfter = (after.Value as IEnumerable<Order>).Count();

            var testOrders = getTestOrders();
            testOrders.RemoveAll(x => x.id == 2);

            Assert.Equal(productsBefore - 1, productsAfter);
            Assert.Equal(testOrders.Count() , productsAfter);
            Assert.Equal(200, result.StatusCode);
        }

        //PUT /api/Orders/Delete/orderId=invalid-order-id
        [Fact]
        public async Task DeleteInvalidOrder_ShouldReturnNoContent()
        {
            var result = await _controller.DeleteOrder(3) as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        //PUT /api/Orders/Delete/productId={productId}
        [Fact]
        public async Task DeleteProductOrder_ShouldReturnWithoutIt()
        {
            var before = await _controller.GetProductsOrdered() as ObjectResult;
            var productsBefore = (before.Value as IEnumerable<OrderItem>).Count();

            var result = await _controller.DeleteProductFromOrder(2) as ObjectResult;

            var after = await _controller.GetProductsOrdered() as ObjectResult;
            var productsAfter = (after.Value as IEnumerable<OrderItem>).Count();
            
            var testProducts = getTestProducts();
            testProducts.RemoveAll(x => x.id == 2);

            Assert.Equal(productsBefore - 1, productsAfter);
            Assert.Equal(testProducts.Count(), productsAfter);
            Assert.Equal(200, result.StatusCode);
            
        }

        //PUT /api/Orders/Delete/productId=invalid-id
        [Fact]
        public async Task DeleteInvalidProductOrder_ShouldReturnSame()
        {
            var result = await _controller.DeleteOrder(3) as NoContentResult;
            Assert.Equal(204, result.StatusCode);
        }

        private List<Order> getTestOrders(string id = "")
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order { id = 1, invoiced = true, dispatched = true, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01"), active = true });
            orders.Add(new Order { id = 2, invoiced = true, dispatched = false, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01"), active = true });
            if (id != "")
            {
                return orders.Where(b => b.buyerId == id && b.active).ToList();
            }
            else
            {
                return orders.Where(b => b.active).ToList();
            }
        }

        private List<OrderItem> getTestProducts(int id = 0 , string buyerId = "")
        {
            List<OrderItem> testProducts = new List<OrderItem>();
            testProducts.Add(new OrderItem {id=1, orderId = 1, name = "Premium Jelly Beans", cost = 2.00, quantity = 5 , active = true });
            testProducts.Add(new OrderItem {id=2, orderId = 2, name = "Netlogo Supercomputer", cost = 2005.99, quantity = 1, active = true });
            if (buyerId != "")
            {
                var listOfOrderIdsByBuyer = getTestOrders().Where(r => r.buyerId == buyerId && r.active).Select(r => r.id);
                var listOfProducts = testProducts.Where(r => listOfOrderIdsByBuyer.Contains(r.orderId) && r.active == true);
                return listOfProducts.ToList();
            }
            else if (id != 0)
            {
                return testProducts.Where(b => b.orderId == id && b.active).ToList();
            }
            else
            {
                return testProducts.Where(b => b.active).ToList();
            }
            
        }
    }
}
