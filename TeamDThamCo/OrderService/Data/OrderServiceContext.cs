using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models
{
    public class OrderServiceContext : DbContext
    {
        public OrderServiceContext (DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<OrderService.Models.Order> Orders { get; set; }
        public DbSet<OrderService.Models.OrderItem> OrderItems { get; set; }
        public DbSet<OrderService.Models.Dispatch> Dispatches { get; set; }
        public DbSet<OrderService.Models.BasketItem> Baskets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
            modelBuilder.Entity<Dispatch>().ToTable("Dispatch");
            modelBuilder.Entity<BasketItem>().ToTable("Baskets");
        }
    }
}
