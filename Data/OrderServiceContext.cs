using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

// A database context for the orderservice db

namespace OrderService.Data
{
    public class OrderServiceContext : DbContext
    {
        public DbSet<OrderService.Models.Order> Orders { get; set; }
        public DbSet<OrderService.Models.OrderItem> OrderItems { get; set; }
        public DbSet<OrderService.Models.Dispatch> Dispatches { get; set; }

        // Constructor
        public OrderServiceContext (DbContextOptions<OrderServiceContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Creates tables when the database is created
        /// </summary>
        /// <param name="modelBuilder">A modelBuilder object</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
            modelBuilder.Entity<Dispatch>().ToTable("Dispatch");
        }
    }
}
