using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Database initializer for the order db

namespace OrderService.Data
{
    public class OrderDbInitializer
    {
        /// <summary>
        /// Called to initialize the database , resets and adds test data if debug.
        /// </summary>
        /// <param name="context">Database context item for order service</param>
        public static void Initialize(OrderServiceContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted(); //Reset for dev
            #endif
            context.Database.EnsureCreated();
            
            #if DEBUG
            List<Order> testOrders = new List<Order>();
            List<OrderItem> testProducts = new List<OrderItem>();

            testOrders.Add(new Order {invoiced = true, dispatched = true, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", paid = true , orderDate = DateTime.Parse("2005-09-01"), active = true });
            testOrders.Add(new Order {invoiced = true, dispatched = false, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", paid = true , orderDate = DateTime.Parse("2005-09-01"), active = true });
            
            testProducts.Add(new OrderItem {productId = 1 , itemName = "Premium Jelly Beans", cost = 2.00m, quantity = 5, active = true });
            testProducts.Add(new OrderItem {productId = 2 , itemName = "Netlogo Supercomputer", cost = 2005.99m, quantity = 1, active = true });
            

            if (context.Orders.Count() == testOrders.Count())
            {
                return;   // DB has been seeded
            }
            else
            {
                #if DEBUG
                context.Orders.AddRange(testOrders);
                context.SaveChanges();

                var orders = context.Orders.Where(b => b.buyerId == "test-id-plz-ignore");
                int counter = 0;

                foreach (OrderItem i in testProducts)
                {
                    testProducts[counter].orderId = orders.ToList()[counter].id;
                    counter++;
                }

                context.OrderItems.AddRange(testProducts);
                context.SaveChanges();

                foreach (Order s in testOrders)
                {
                    if (s.dispatched)
                    {
                        context.Dispatches.Add(new Dispatch { orderId = s.id });
                    }
                }
                context.SaveChanges();
                #endif
            }
            #endif

            return;
        }
    }
}
