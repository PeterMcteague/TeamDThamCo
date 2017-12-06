using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestockService.Infrastructure
{
    public class RestockDbInitializer
    {
        public static void Initialize(RestockContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted();
            #endif
            context.Database.EnsureCreated();
            #if DEBUG
            context.Suppliers.Add(new Models.Supplier { Name = "Dodgydealers", GetUri = new Uri("http://dodgydealers.azurewebsites.net/api/product/"), OrderUri = new Uri("http://dodgydealers.azurewebsites.net/api/Order/Id={Id}&AccountName={AccountName}&CardNumber={CardNumber}&ProductId={ProductId}&Quantity={Quantity}&When={When}&ProductName={ProductName}&ProductEan={ProductEan}&TotalPrice={TotalPrice}") });
            context.Cards.Add(new Models.Card { CardNumber = "1111111111111111", AccountName = "Peter's account" });
            context.SaveChanges();
            #endif
        }
    }
}
