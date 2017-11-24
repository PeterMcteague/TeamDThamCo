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
            testInvoices.Add(new InvoiceItem {customerId = "test-id-plz-ignore" , orderIds  = new List<int>(new int[] {1}) });
            testInvoices.Add(new InvoiceItem {customerId = "test-id-plz-ignore" , orderIds = new List<int>(new int[] {2}) });
            #endif

            if (context.Invoices.Count() == testInvoices.Count())
            {
                return;   // DB has been seeded
            }
            else
            {
                #if DEBUG
                context.Invoices.AddRange(testInvoices);
                context.SaveChanges();
                #endif
            }
        }
    }
}
