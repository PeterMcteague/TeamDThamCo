using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data
{
    public class HangfireContext : DbContext
    {
        public HangfireContext()
        {
        }

        public HangfireContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
