using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using BasketService.Models;
using Microsoft.EntityFrameworkCore;
using BasketService.Controllers;
using Microsoft.AspNetCore.Mvc;
using BasketService.Data;

namespace BasketService.Test
{
    public class BasketApiControllerTest
    {
        private BasketContext _context;
        private BasketController _controller;

        public BasketApiControllerTest()
        {
            // Initialize DbContext in memory
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("BasketAPITestDB");
            _context = new BasketContext(optionsBuilder.Options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            
            // Seed data
            List<BasketItem> testBasket = new List<BasketItem>();
            testBasket.Add(new BasketItem {buyerId = "test-id-plz-ignore", productId = 1, quantity = 9001 });
            testBasket.Add(new BasketItem {buyerId = "test-id-plz-ignore", productId = 2, quantity = 1 });
            testBasket.Add(new BasketItem {buyerId = "another-test-id", productId = 2, quantity = 1 });

            _context.Baskets.AddRange(testBasket);
            _context.SaveChanges();

            // Create test subject
            _controller = new BasketController(_context);
        }

        //GetBasket
        [Fact]
        public async Task GetBaskets_ShouldReturnBaskets()
        {
            var result = await _controller.GetBaskets() as ObjectResult;
            var basket = result.Value as IEnumerable<BasketItem>;

            Assert.Equal(getTestBasket().Count(), basket.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GetBasket
        [Fact]
        public async Task GetBasketId_ShouldReturnBasket()
        {
            var result = await _controller.GetBasket("test-id-plz-ignore") as ObjectResult;
            var basket = result.Value as IEnumerable<BasketItem>;

            Assert.Equal(getTestBasket().Count() - 1, basket.Count());
            Assert.Equal(200, result.StatusCode);
        }

        //GetBasketItem
        [Fact]
        public async Task GetBasketItemByIDAndProductId_ShouldReturnBasketItem()
        {
            var result = await _controller.GetBasketItem("test-id-plz-ignore",1) as ObjectResult;
            var basket = result.Value as IEnumerable<BasketItem>;

            Assert.Single(basket);
            Assert.Equal("test-id-plz-ignore", basket.FirstOrDefault().buyerId);
            Assert.Equal(9001, basket.FirstOrDefault().quantity);
            Assert.Equal(1, basket.FirstOrDefault().productId);
            Assert.Equal(200, result.StatusCode);
        }

        //AddItemToBasket
        [Fact]
        public async Task AddBasket_ShouldAddItem()
        {
            var result = await _controller.AddItemToBasket("test-id-plz-ignore",3,500) as ObjectResult;
            var itemAdded = result.Value as BasketItem;

            var result2 = await _controller.GetBaskets() as ObjectResult;
            var basket = result2.Value as IEnumerable<BasketItem>;

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("test-id-plz-ignore" , itemAdded.buyerId);
            Assert.Equal(3, itemAdded.productId);
            Assert.Equal(500, itemAdded.quantity);
            Assert.Equal(getTestBasket().Count() + 1, basket.Count());
        }

        //UpdateItemInBasket
        [Fact]
        public async Task UpdateBasket_ShouldUpdateItem()
        {
            var before = await _controller.GetBasketItem("test-id-plz-ignore", 2) as ObjectResult;
            var beforeItem = before.Value as IEnumerable<BasketItem>;
            var beforeItemQuantity = beforeItem.FirstOrDefault().quantity;

            var after = await _controller.UpdateItemInBasket("test-id-plz-ignore", 2 , 26) as ObjectResult;
            var afterItem = after.Value as BasketItem;
            var afterItemQuantity = afterItem.quantity;

            Assert.Equal(200, after.StatusCode);
            Assert.Equal(getTestBasket().Where(b=>b.buyerId=="test-id-plz-ignore" && b.productId==2).FirstOrDefault().quantity, beforeItemQuantity); //Check quantity as expected before
            Assert.Equal(26,afterItemQuantity); //Check quantity as expected after
        }

        //DeleteBasketItem
        [Fact]
        public async Task DeleteItem_ShouldRemoveItem()
        {
            var before = await _controller.GetBasketItem("test-id-plz-ignore", 2) as ObjectResult;
            var beforeItem = before.Value as IEnumerable<BasketItem>;

            var deleted = await _controller.DeleteBasketItem("test-id-plz-ignore", 2) as ObjectResult;
            var deletedItem = deleted.Value as BasketItem;

            var after = await _controller.GetBasketItem("test-id-plz-ignore", 2) as NotFoundResult;

            var getAfter = await _controller.GetBaskets() as ObjectResult;
            var afterItems = getAfter.Value as IEnumerable<BasketItem>;

            Assert.Equal(404, after.StatusCode);
            Assert.Equal(beforeItem.FirstOrDefault() , deletedItem); 
            Assert.Equal(getTestBasket().Count() - 1, afterItems.Count()); 
        }

        private List<BasketItem> getTestBasket(string id = "")
        {
            List<BasketItem> basket = new List<BasketItem>();
            basket.Add(new BasketItem { id = 1, buyerId = "test-id-plz-ignore", productId = 1, quantity = 9001 });
            basket.Add(new BasketItem { id = 2, buyerId = "test-id-plz-ignore", productId = 2, quantity = 1 });
            basket.Add(new BasketItem { id = 3, buyerId = "another-test-id", productId = 2, quantity = 1 });
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
