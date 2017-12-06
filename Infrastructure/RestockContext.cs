using Microsoft.EntityFrameworkCore;
using RestockService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestockService
{
    public class RestockContext : DbContext
    {
        public RestockContext(DbContextOptions<RestockContext> options) : base(options)
        {
        }
        
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>().ToTable("Supplier");
            modelBuilder.Entity<Card>().ToTable("Card");
        }

    }
}
