using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Models
{
    public class InvoiceOrder
    {
        [Key, Column("id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int invoiceId { get; set; }

        public int orderId { get; set; }

        public double cost { get; set; }
    }
}
