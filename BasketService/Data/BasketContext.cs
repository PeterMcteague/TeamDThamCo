using BasketService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketService.Data
{
    public class BasketContext : DbContext
    {
        public BasketContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<BasketService.Models.BasketItem> Baskets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasketItem>().ToTable("Baskets");
        }
    }
}
