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
            context.Database.EnsureCreated();

            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }
            
            List<OrderItem> testProducts = new List<OrderItem>();
            testProducts.Add(new OrderItem { orderId = 1, name = "Premium Jelly Beans", cost = 2.00, quantity = 5 });
            

            var orders = new Order[]
            {
                new Order{id=1,invoiced=true,dispatched=true, address="Kevins House, 69 Wallaby Way, Sydney, PST CDE", buyerId="test-id-plz-ignore",orderDate=DateTime.Parse("2005-09-01")}
            };
            foreach (Order s in orders)
            {
                context.Orders.Add(s);
            }
            foreach (OrderItem s in testProducts)
            {
                context.OrderItems.Add(s);
            }
            foreach (Order s in orders)
            {
                if (s.dispatched)
                {
                    context.Dispatches.Add(new Dispatch { orderId = s.id });
                }
            }
            context.SaveChanges();
        }
    }
}
