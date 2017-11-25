using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Models
{
    public class InvoiceItem
    {
        [Key, Column("id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string customerId { get; set; } //The customer that the invoice belongs to
    }
}
