using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasketService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BasketService.Data
{
    public class BasketDbInitializer
    {
        public static void Initialize(BasketContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted(); //Reset for dev
            #endif
            context.Database.EnsureCreated();
            
            #if DEBUG
            // Seed data
            List<BasketItem> testBasket = new List<BasketItem>();
            testBasket.Add(new BasketItem {buyerId = "test-id-plz-ignore", productId = 1, quantity = 9001 });
            testBasket.Add(new BasketItem {buyerId = "test-id-plz-ignore", productId = 2, quantity = 1 });

            context.Baskets.AddRange(testBasket);
            context.SaveChanges();
            #endif

            if (context.Baskets.Count() == testBasket.Count())
            {
                return;   // DB has been seeded
            }
            else
            {
                #if DEBUG
                context.Baskets.AddRange(testBasket);
                context.SaveChanges();
                #endif
            }
        }
    }
}
