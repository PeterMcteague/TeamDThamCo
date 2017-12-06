using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasketService.Models;

// Database initializer for the basket db

namespace BasketService.Data
{
    public class BasketDbInitializer
    {
        /// <summary>
        /// Called to setup the database
        /// </summary>
        /// <param name="context">The database context</param>
        public static void Initialize(BasketContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted(); //Reset for dev
            #endif

            context.Database.EnsureCreated();
            
            #if DEBUG
            // Seed data
            List<BasketItem> testBasket = new List<BasketItem>();
            testBasket.Add(new BasketItem {name = "Premium Jelly Beans", cost = 0.80m, buyerId = "test-id-plz-ignore", productId = 1, quantity = 5});
            testBasket.Add(new BasketItem {name = "Netlogo Supercomputer", cost = 2005.99m, buyerId = "test-id-plz-ignore", productId = 2, quantity = 1});
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
            #endif
            return;
        }
    }
}
