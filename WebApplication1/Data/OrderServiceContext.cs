using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class OrderServiceContext : DbContext
    {
        private string v;

        public OrderServiceContext (DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<OrderService.Models.Order> Orders { get; set; }
        public DbSet<OrderService.Models.OrderItem> OrderItems { get; set; }
        public DbSet<OrderService.Models.Dispatch> Dispatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
            modelBuilder.Entity<Dispatch>().ToTable("Dispatch");
        }
    }
}
