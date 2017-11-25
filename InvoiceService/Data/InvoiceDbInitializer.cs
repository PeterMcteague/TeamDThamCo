using InvoiceService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Data
{
    public class InvoiceDbInitializer
    {
        public static void Initialize(InvoiceContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted(); //Reset for dev
            #endif
            context.Database.EnsureCreated();

            #if DEBUG
            // Seed data
            List<InvoiceItem> testInvoices = new List<InvoiceItem>();
            testInvoices.Add(new InvoiceItem {customerId = "test-id-plz-ignore"});
            testInvoices.Add(new InvoiceItem { customerId = "test-id-plz-ignore"});
            List<InvoiceOrder> testInvoiceOrders = new List<InvoiceOrder>();
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 1,orderId = 1, cost=2.00});
            testInvoiceOrders.Add(new InvoiceOrder { invoiceId = 2, orderId = 2, cost =2005.99 });
            #endif

            if (context.Invoices.Count() == testInvoices.Count())
            {
                return;   // DB has been seeded
            }
            else
            {
                #if DEBUG
                context.Invoices.AddRange(testInvoices);
                context.InvoiceOrders.AddRange(testInvoiceOrders);
                context.SaveChanges();
                #endif
            }
        }
    }
}
