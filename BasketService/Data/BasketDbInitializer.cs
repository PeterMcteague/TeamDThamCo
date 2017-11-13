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

            List<BasketItem> testBasket = new List<BasketItem>();

            #if DEBUG
            testBasket.Add(new BasketItem { id = 1, buyerId = "test-id-plz-ignore", productId = 1, quantity = 9001 });
            #endif

            if (context.Baskets.Count() == testBasket.Count())
            {
                return;   // DB has been seeded
            }
            else
            {
                context.Baskets.AddRange(testBasket);
                context.SaveChanges();
            }
        }
    }
}
