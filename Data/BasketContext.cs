using BasketService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// A entity framework dbcontext for the basket database

namespace BasketService.Data
{
    public class BasketContext : DbContext
    {
        public DbSet<BasketService.Models.BasketItem> Baskets { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Database context options for the dbcontext</param>
        public BasketContext(DbContextOptions<BasketContext> options)
            : base(options)
        {
        }
        
        /// <summary>
        /// A function ran when the model is created , adds tables to the db
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder object passed when creating the database</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasketItem>().ToTable("Baskets");
        }
    }
}
