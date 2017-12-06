using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Initalizes the hangfire database

namespace OrderService.Data
{
    public class HangfireDbInitializer
    {
        // Called to intiaialize the database
        public static void Initialize(HangfireContext context)
        {
            #if DEBUG
            context.Database.EnsureDeleted(); //Reset for dev
            #endif
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}
