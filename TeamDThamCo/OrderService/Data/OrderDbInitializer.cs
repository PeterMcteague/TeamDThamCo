using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data
{
    public class OrderDbInitializer
    {
        public static void Initialize(OrderServiceContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted(); //Reset for dev
            #endif
            context.Database.EnsureCreated();
            
            List<Order> testOrders = new List<Order>();
            List<OrderItem> testProducts = new List<OrderItem>();

            #if DEBUG
            testOrders.Add(new Order { invoiced = true, dispatched = true, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01"), active = true });
            testOrders.Add(new Order { invoiced = true, dispatched = false, address = "Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId = "test-id-plz-ignore", orderDate = DateTime.Parse("2005-09-01"), active = true });
            
            testProducts.Add(new OrderItem { orderId = testOrders[0].id, name = "Premium Jelly Beans", cost = 2.00, quantity = 5, active = true });
            testProducts.Add(new OrderItem { orderId = testOrders[1].id, name = "Netlogo Supercomputer", cost = 2005.99, quantity = 1, active = true });
            #endif

            if (context.Orders.Count() == testOrders.Count())
            {
                return;   // DB has been seeded
            }
            else
            {
                #if DEBUG
                context.Orders.AddRange(testOrders);
                context.SaveChanges();

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
        }
    }
}
