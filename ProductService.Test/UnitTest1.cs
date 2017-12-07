using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductService.Controllers;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductService.Test
{
    public class UnitTest1
    {
        public class ProductApiControllerTest : IDisposable
        {
            private ProductContext _context;
            private ProductController _controller;
            private SqliteConnection connection;
            private DateTime testOclock;

            public ProductApiControllerTest()
            {
                // Initialize DbContext
                connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                var optionsBuilder = new DbContextOptionsBuilder<ProductContext>();
                optionsBuilder.UseSqlite(connection);
                _context = new ProductContext(optionsBuilder.Options);
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();

                // Seed data
                testOclock = DateTime.Now;
                _context.Products.Add(new Models.Product { Id = 1, BrandId = 1, BrandName = "iStuff-R-Us", CategoryId = 1, CategoryName = "Screen Protectors", Description = "For his or her sensory pleasure. Fits few known smartphones.", Ean = "5 102310 300410", ExpectedRestock = DateTime.Now, InStock = true, StockNumber = 1, Name = "Rippled Screen Protector", Price = 7.94m });
                _context.PriceLogs.Add(new Models.PriceLog { Id = 1, productId = 1, OldPrice = 8.79m, UpdateDate = testOclock });

                _context.SaveChanges();

                // Create test subject
                _controller = new ProductController(_context);
            }

            //GET /api/Products
            [Fact]
            public async Task GetProductById_ShouldReturnProduct()
            {
                var result = await _controller.GetItemById(1) as ObjectResult;
                var products = result.Value as Product;

                Assert.Equal(getTestProducts()[0].BrandName, products.BrandName);
                Assert.Equal(200, result.StatusCode);
            }

            [Fact]
            public async Task GetNonExistentProduct_ThrowError()
            {
                var result = await _controller.GetItemById(999999) as NotFoundResult;
                
                Assert.Equal(404, result.StatusCode);
            }

            private List<Product> getTestProducts()
            {
                List<Product> testProducts = new List<Product>();
                testProducts.Add(new Models.Product { Id = 1, BrandId = 1, BrandName = "iStuff-R-Us", CategoryId = 1, CategoryName = "Screen Protectors", Description = "For his or her sensory pleasure. Fits few known smartphones.", Ean = "5 102310 300410", ExpectedRestock = DateTime.Now, InStock = true, StockNumber = 1, Name = "Rippled Screen Protector", Price = 7.94m });
                return testProducts;
            }

            private List<PriceLog> getTestPriceLog()
            {
                List<PriceLog> testPriceLog = new List<PriceLog>();
                testPriceLog.Add(new Models.PriceLog { Id = 1, productId = 1, OldPrice = 8.79m, UpdateDate = testOclock });
                return testPriceLog;
            }

            public void Dispose()
            {
                connection.Close();
            }
        }
    }
}
