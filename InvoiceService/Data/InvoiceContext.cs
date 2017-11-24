using InvoiceService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Data
{
    public class InvoiceContext : DbContext
    {
        public InvoiceContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<InvoiceService.Models.InvoiceItem> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceItem>().ToTable("Invoices");
        }
    }
}
