using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class OrderItem
    {
        [Key, Column("id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [ForeignKey("orderId")]
        public int orderId { get; set; }

        [ForeignKey("productId")]
        public int productId { get; set; }

        public string itemName { get; set; }

        public int quantity { get; set; }

        // Refers to cost of 1 item, not many
        public decimal cost { get; set; }

        public Boolean active { get; set; }
    }
}
