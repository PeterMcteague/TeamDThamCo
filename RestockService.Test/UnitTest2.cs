using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RestockService.Controllers;
using RestockService.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RestockService.Test
{
    public class UnitTest2
    {
        public class RestockApiControllerTest : IDisposable
        {
            private RestockContext _context;
            private RestockController _controller;
            private SqliteConnection connection;
            private DateTime testOclock;

            public RestockApiControllerTest()
            {
                // Initialize DbContext
                connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                var optionsBuilder = new DbContextOptionsBuilder<RestockContext>();
                optionsBuilder.UseSqlite(connection);
                _context = new RestockContext(optionsBuilder.Options);
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();

                // Seed data
                testOclock = DateTime.Now;
                _context.Cards.Add(new Models.Card { CardNumber = "1111111111111111" , AccountName = "Peter's account" });
                _context.Suppliers.Add(new Models.Supplier { Id = 1, Name = "Dodgydealers", GetUri = "http://dodgydealers.azurewebsites.net/api/product/", OrderUri = "http://dodgydealers.azurewebsites.net/api/Order/Id={Id}&AccountName={AccountName}&CardNumber={CardNumber}&ProductId={ProductId}&Quantity={Quantity}&When={When}&ProductName={ProductName}&ProductEan={ProductEan}&TotalPrice={TotalPrice}" });

                _context.SaveChanges();

                // Create test subject
                _controller = new RestockController(_context);
            }

            //GET /api/Products
            [Fact]
            public async Task GetSupplierProductById_ShouldReturnProduct()
            {
                Supplier s = new Supplier(){Id = 1 , Name = "Dodgydealers", GetUri = "http://dodgydealers.azurewebsites.net/api/product/", OrderUri = "http://dodgydealers.azurewebsites.net/api/Order/Id={Id}&AccountName={AccountName}&CardNumber={CardNumber}&ProductId={ProductId}&Quantity={Quantity}&When={When}&ProductName={ProductName}&ProductEan={ProductEan}&TotalPrice={TotalPrice}" };
                var result = _controller.GetSupplierProductById(1, s) as ObjectResult;
                var products = result.Value as Product;

                Assert.Equal(true, products.BrandName != null);
                Assert.Equal(200, result.StatusCode);
            }

            [Fact]
            public async Task GetNonExistentProduct_ThrowError()
            {
                Supplier s = new Supplier() { Id = 1, Name = "Dodgydealers", GetUri = "http://dodgydealers.azurewebsites.net/api/product/", OrderUri = "http://dodgydealers.azurewebsites.net/api/Order/Id={Id}&AccountName={AccountName}&CardNumber={CardNumber}&ProductId={ProductId}&Quantity={Quantity}&When={When}&ProductName={ProductName}&ProductEan={ProductEan}&TotalPrice={TotalPrice}" };
                var result = _controller.GetSupplierProductById(999999, s) as NotFoundResult;

                Assert.Equal(404, result.StatusCode);
            }

            [Fact]
            public async Task GetNonExistentSupplier_ThrowError()
            {
                Supplier s = new Supplier() { Id = 1, Name = "NonExistentSupplier", GetUri = "http://somesite/", OrderUri = "http://somesite" };
                var result = _controller.GetSupplierProductById(1, s) as NotFoundResult;

                Assert.Equal(404, result.StatusCode);
            }

            public void Dispose()
            {
                connection.Close();
            }
        }
    }
}
