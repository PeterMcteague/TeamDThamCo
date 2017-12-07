using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Infrastructure
{
    public class ProductDbInitializer
    {
        public static void Initialize(ProductContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted();
            #endif
            context.Database.EnsureCreated();
            #if DEBUG
            context.Products.Add(new Models.Product { BrandId = 1, BrandName = "iStuff-R-Us", CategoryId = 1, CategoryName = "Screen Protectors", Description = "For his or her sensory pleasure. Fits few known smartphones.", Ean = "5 102310 300410", ExpectedRestock = DateTime.Now, InStock = true, StockNumber = 1, Name = "Rippled Screen Protector", Price = 7.94m });
            context.PriceLogs.Add(new Models.PriceLog { Id = 2, Name = "Peter's account", OldPrice = 8.79m, UpdateDate = DateTime.Now });
            context.SaveChanges();
            #endif
        }
    }
}
