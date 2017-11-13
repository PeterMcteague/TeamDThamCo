using OrderService.Models;
using OrderService.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace OrderService.Test
{
    public class BasketApiControllerTest
    {
        private OrderServiceContext _context;
        private BasketAPIController _controller;

        public BasketApiControllerTest()
        {
            // Initialize DbContext in memory
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("BasketAPITestDB");
            _context = new OrderServiceContext(optionsBuilder.Options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            // Seed data
            List<BasketItem> testBasket = new List<BasketItem>();
            testBasket.Add(new BasketItem { id = 1, buyerId = "test-id-plz-ignore", productId = 1, quantity = 9001 });
            testBasket.Add(new BasketItem { id = 2, buyerId = "test-id-plz-ignore", productId = 2, quantity = 1 });

            _context.Baskets.AddRange(testBasket);
            _context.SaveChanges();

            // Create test subject
            _controller = new BasketAPIController(_context);
        }

        //GetBasket
        [Fact]
        public async Task GetBasket_ShouldReturnBasket()
        {
            var result = await _controller.GetBasket("test-id-plz-ignore") as ObjectResult;
            var basket = result.Value as IEnumerable<BasketItem>;

            Assert.Equal(getTestBasket().Count(), basket.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //AddItemToBasket
        //UpdateItemInBasket
        //DeleteBasketItem

        private List<BasketItem> getTestBasket(string id = "")
        {
            List<BasketItem> basket = new List<BasketItem>();
            basket.Add(new BasketItem { id = 1, buyerId = "test-id-plz-ignore", productId = 1, quantity = 9001 });
            basket.Add(new BasketItem { id = 2, buyerId = "test-id-plz-ignore", productId = 2, quantity = 1 });
            if (id != "")
            {
                return basket.Where(b => b.buyerId == id).ToList();
            }
            else
            {
                return basket.ToList();
            }
        }
    }
}
