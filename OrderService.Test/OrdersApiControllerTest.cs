using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using OrderService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OrderService.Controllers;
using Microsoft.Data.Sqlite;

namespace OrderService.Test
{
    public class OrdersApiControllerTest : IDisposable
    {
        private OrderServiceContext _context;
        private OrdersAPIController _controller;
        private SqliteConnection connection;

        public OrdersApiControllerTest()
        {
            // Initialize DbContext
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlite(connection);
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
            var productsBefore = _context.Orders.AsNoTracking().Where(b => b.active == true).Count();

            var result = await _controller.DeleteOrder(2) as ObjectResult;

            var productsAfter = _context.Orders.AsNoTracking().Where(b => b.active == true).Count();

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
            var productsBefore = _context.OrderItems.AsNoTracking().Where(b => b.active == true).Count();

            var result = await _controller.DeleteProductFromOrder(2) as ObjectResult;

            var productsAfter = _context.OrderItems.AsNoTracking().Where(b => b.active == true).Count();

            var testProducts = getTestProducts();
            testProducts.RemoveAll(x => x.id == 2);

            Assert.Equal(productsBefore - 1, productsAfter);
            Assert.Equal(testProducts.Count(), productsAfter);
            Assert.Equal(200, result.StatusCode);
        }

        //PUT /api/Orders/Add/buyerId={buyerId}&address={address}
        [Fact]
        public async Task AddOrder_ShouldReturnOrderAdded()
        {
            var productsBefore = _context.Orders.AsNoTracking().Count();

            var result = await _controller.AddOrder("another-id", "Peters House, Street, County, PostCode") as ObjectResult;
            var resultItem = result.Value as Order;
            
            var productsAfter = _context.Orders.AsNoTracking().Count();

            Assert.Equal(productsBefore + 1, productsAfter);
            Assert.Equal(200, result.StatusCode);
        }

        //PUT /api/Orders/Add/buyerId={buyerId}&address={address}
        [Fact]
        public async Task AddOrderItem_ShouldAddItemToOrder()
        {
            //Getting count before add
            var beforeCount = _context.OrderItems.AsNoTracking().Where(b => b.active == true).Count();
            //Test that response is 200 for valid
            var validRequest = await _controller.AddOrderItem(1,"Blue cheese 1KG",1,9.00) as ObjectResult;
            var validRequestItem = validRequest.Value as OrderItem;
            //Getting count after add
            var afterCount = _context.OrderItems.AsNoTracking().Where(b => b.active == true).Count();
            //Test that response is 404 for invalid orderId
            var invalidRequest = await _controller.AddOrderItem(7, "Blue cheese 1KG", 1, 9.00) as NotFoundResult;

            Assert.Equal(beforeCount + 1, afterCount);
            Assert.Equal(200, validRequest.StatusCode);
            Assert.Equal(404, invalidRequest.StatusCode);
            Assert.Equal(1, validRequestItem.orderId);
            Assert.Equal("Blue cheese 1KG", validRequestItem.name);
            Assert.Equal(1, validRequestItem.quantity);
            Assert.Equal(9.00, validRequestItem.cost);
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

        public void Dispose()
        {
            connection.Close();
        }
    }
}
