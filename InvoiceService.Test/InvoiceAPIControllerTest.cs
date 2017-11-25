using InvoiceService.Controllers;
using InvoiceService.Data;
using InvoiceService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InvoiceService.Test
{
    public class InvoiceAPIControllerTest : IDisposable
    {
        private InvoiceContext _context;
        private InvoiceAPIController _controller;
        private SqliteConnection connection;

        public InvoiceAPIControllerTest()
        {
            // Initialize DbContext in memory
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlite(connection);
            _context = new InvoiceContext(optionsBuilder.Options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed data
            // Seed data
            List<InvoiceItem> testInvoices = new List<InvoiceItem>();
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore" });
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore" });
            testInvoices.Add(new InvoiceItem { customerId = "another-id" });
            List<InvoiceOrder> testInvoiceOrders = new List<InvoiceOrder>();
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 1, orderId = 1, cost = 2.00 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 2, cost = 2005.99 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 4, cost = 20.00 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 5, cost = 29.99 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 3, orderId = 3, cost = 199 });

            _context.Invoices.AddRange(testInvoices);
            _context.InvoiceOrders.AddRange(testInvoiceOrders);
            _context.SaveChanges();

            // Create test subject
            _controller = new InvoiceAPIController(_context);
        }
        
        [Fact]
        public async Task GetInvoices_ShouldReturnInvoices()
        {
            var result = await _controller.GetInvoices() as ObjectResult;
            var invoices = result.Value as IEnumerable<InvoiceItem>;

            Assert.Equal(getTestInvoices().Count(), invoices.Count());
            Assert.Equal(200, result.StatusCode);
        }
        
        [Fact]
        public async Task GetXInvoices_ShouldReturnXInvoices()
        {
            var result = await _controller.GetInvoices(1) as ObjectResult;
            var invoices = result.Value as IEnumerable<InvoiceItem>;

            Assert.Equal(1, invoices.Count());
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetCustomerIDInvoices_ShouldReturnTheirInvoices()
        {
            var validResult = await _controller.GetInvoices("test-id-plz-ignore") as ObjectResult;
            var validResultCount = (validResult.Value as IEnumerable<InvoiceItem>).Count();

            var invalidResult = await _controller.GetInvoices("space-ghost") as NotFoundResult;
            var invalidResultCode= invalidResult.StatusCode;

            Assert.Equal(getTestInvoices().Count() - 1, validResultCount);
            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(404, invalidResultCode);
        }

        [Fact]
        public async Task GetXCustomerIDInvoices_ShouldReturnXOfTheirInvoices()
        {
            var validResult = await _controller.GetInvoices(1,"test-id-plz-ignore") as ObjectResult;
            var validResultCount = (validResult.Value as IEnumerable<InvoiceItem>).Count();

            var invalidResult = await _controller.GetInvoices(1,"space-ghost") as NotFoundResult;
            var invalidResultCode = invalidResult.StatusCode;

            Assert.Equal(1, validResultCount);
            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(404, invalidResultCode);
        }

        [Fact]
        public async Task GetInvoiceOrders_ShouldReturnInvoiceOrders()
        {
            var validResult = await _controller.GetInvoiceOrders(1) as ObjectResult;
            var validResultValue = (validResult.Value as IEnumerable<InvoiceOrder>);
            var validResultFirstValue = validResultValue.FirstOrDefault();

            var invalidResult = await _controller.GetInvoiceOrders(9999) as NotFoundResult;
            var invalidResultCode = invalidResult.StatusCode;

            Assert.Equal(2.00, validResultFirstValue.cost);
            Assert.Equal(1, validResultFirstValue.invoiceId);
            Assert.Equal(1, validResultFirstValue.orderId);
            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(404, invalidResultCode);
        }

        [Fact]
        public async Task GetXInvoiceOrders_ShouldReturnXInvoiceOrders()
        {
            var validResult = await _controller.GetInvoiceOrders(2,2) as ObjectResult;
            var validResultValue = (validResult.Value as IEnumerable<InvoiceOrder>);

            var validResultWithLessQuantity = await _controller.GetInvoiceOrders(1, 2) as ObjectResult;
            var validResultWithLessQuantityValue = (validResultWithLessQuantity.Value as IEnumerable<InvoiceOrder>);

            var invalidResult = await _controller.GetInvoiceOrders(9999,1) as NotFoundResult;
            var invalidResultCode = invalidResult.StatusCode;

            Assert.Equal(2 , validResultValue.Count());
            Assert.Equal(1, validResultWithLessQuantityValue.Count());
            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(404, invalidResultCode);
        }

        [Fact]
        public async Task GetInvoiceOrdersById_ReturnsTheInvoiceOrderWithThatOrderId()
        {
            var validResult = await _controller.GetInvoiceOrderByOrderid(2) as ObjectResult;
            var validResultValue = (validResult.Value as InvoiceOrder);

            var invalidResult = await _controller.GetInvoiceOrderByOrderid(9999) as NotFoundResult;
            var invalidResultCode = invalidResult.StatusCode;

            Assert.Equal(2005.99, validResultValue.cost);
            Assert.Equal(2, validResultValue.invoiceId);
            Assert.Equal(2, validResultValue.orderId);
            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(404, invalidResultCode);
        }

        [Fact]
        public async Task AddInvoice_ShouldAddAndReturnTheInvoice()
        {
            var getResultBefore = await _controller.GetInvoices("test-id") as NotFoundResult;
            var getResultCode = getResultBefore.StatusCode;

            var validResult = await _controller.AddInvoice("test-id",new List<int> {4,5},new List<double> { 0.50, 0.50 }) as ObjectResult;
            var validResultReturn = (validResult.Value as InvoiceItem);

            var getResultAfter = await _controller.GetInvoices("test-id") as ObjectResult;
            var getResultAfterItem = (getResultAfter.Value as IEnumerable<InvoiceItem>).FirstOrDefault();
            var getResultAfterCode = getResultAfter.StatusCode;

            var invoiceItemsAfter = await _controller.GetInvoiceOrders(getResultAfterItem.id) as ObjectResult;
            var invoiceItemsAfterOrders = (invoiceItemsAfter.Value as IEnumerable<InvoiceOrder>);

            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(200, getResultAfterCode);
            Assert.Equal(404, getResultCode);
            Assert.Equal("test-id", validResultReturn.customerId);
            Assert.Equal("test-id", getResultAfterItem.customerId);
            Assert.Equal(4, invoiceItemsAfterOrders.ElementAt(0).orderId);
            Assert.Equal(5, invoiceItemsAfterOrders.ElementAt(1).orderId);
        }

        private List<InvoiceItem> getTestInvoices()
        {
            List<InvoiceItem> testInvoices = new List<InvoiceItem>();
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore" });
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore" });
            testInvoices.Add(new InvoiceItem { customerId = "another-id" });
            return testInvoices.ToList();
        }

        private List<InvoiceOrder> getTestInvoiceOrders()
        {
            List<InvoiceOrder> testInvoiceOrders = new List<InvoiceOrder>();
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 1, orderId = 1, cost = 2.00 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 2, cost = 2005.99 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 4, cost = 20.00 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 5, cost = 29.99 });
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 3, orderId = 3, cost = 199 });
            return testInvoiceOrders.ToList();
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
