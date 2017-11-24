using InvoiceService.Controllers;
using InvoiceService.Data;
using InvoiceService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InvoiceService.Test
{
    public class InvoiceAPIControllerTest
    {
        private InvoiceContext _context;
        private InvoiceAPIController _controller;

        public InvoiceAPIControllerTest()
        {
            // Initialize DbContext in memory
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("InvoiceAPITestDb");
            _context = new InvoiceContext(optionsBuilder.Options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed data
            List<InvoiceItem> testInvoices = new List<InvoiceItem>();
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore", orderIds = new List<int>(new int[] { 1 }), cost = 2.00 });
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore", orderIds = new List<int>(new int[] { 2 }), cost = 2005.99 });
            testInvoices.Add(new InvoiceItem { customerId = "another-id", orderIds = new List<int>(new int[] { 3 }), cost = 199 });

            _context.Invoices.AddRange(testInvoices);
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

            Assert.Equal(getTestInvoices().Count() - 1, invoices.Count());
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
        public async Task GetXCustomerIDInvoices_ShouldReturnTheirInvoices()
        {
            var validResult = await _controller.GetInvoices(1,"test-id-plz-ignore") as ObjectResult;
            var validResultCount = (validResult.Value as IEnumerable<InvoiceItem>).Count();

            Assert.Equal(getTestInvoices().Where(b => b.customerId == "test-id-plz-ignore").SingleOrDefault().orderIds, (validResult.Value as InvoiceItem).orderIds);
            Assert.Equal(1, validResultCount);
            Assert.Equal(200, validResult.StatusCode);
        }

        [Fact]
        public async Task AddInvoice_ShouldAddAndReturnTheInvoice()
        {
            var validResult = await _controller.AddInvoice("test-id",1.00,new List<int> {4,5 }) as ObjectResult;
            var validResultReturn = (validResult.Value as InvoiceItem);
            var getResultBefore = await _controller.AddInvoice("test-id", 1.00, new List<int> { 4, 5 }) as NotFoundResult;
            var getResult = await _controller.GetInvoices("test-id") as ObjectResult;
            var getResultReturn = (getResult.Value as InvoiceItem);
            
            Assert.Equal(200, validResult.StatusCode);
            Assert.Equal(404, getResultBefore.StatusCode);
            Assert.Equal("test-id", validResultReturn.customerId);
            Assert.Equal("test-id", getResultReturn.customerId);
            Assert.Equal(new List<int> { 4, 5 }, getResultReturn.orderIds);
        }

        private List<InvoiceItem> getTestInvoices()
        {
            List<InvoiceItem> testInvoices = new List<InvoiceItem>();
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore", orderIds = new List<int>(new int[] { 1 }), cost = 2.00 });
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore", orderIds = new List<int>(new int[] { 2 }), cost = 2005.99 });
            testInvoices.Add(new InvoiceItem { customerId = "another-id", orderIds = new List<int>(new int[] { 3 }), cost = 199 });
            return testInvoices.ToList();
        }
    }
}
