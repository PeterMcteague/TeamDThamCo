using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Tests
{
    [TestClass]
    class TestOrdersAPIController
    {
        //GET /api/Orders
        [TestMethod]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            var testProducts = GetTestProducts();
            var controller = new OrdersApiController(testProducts);

            var result = await controller.GetAllProductsAsync() as List<Product>;
            Assert.AreEqual(testProducts.Count, result.Count);
        }

        //GET /api/Orders/{id}
        //GET /api/Orders/Products
        //GET /api/Orders/Products/{id}

        private List<OrderService.Models.Orders> GetTestProducts()
        {
            var testProducts = new List<Product>();
            testProducts.Add(new Product { Id = 1, Name = "Demo1", Price = 1 });
            testProducts.Add(new Product { Id = 2, Name = "Demo2", Price = 3.75M });
            testProducts.Add(new Product { Id = 3, Name = "Demo3", Price = 16.99M });
            testProducts.Add(new Product { Id = 4, Name = "Demo4", Price = 11.00M });

            return testProducts;
        }
    }
}
