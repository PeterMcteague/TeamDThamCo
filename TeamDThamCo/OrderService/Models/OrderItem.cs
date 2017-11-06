using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class OrderItem
    {
        public int id { get; set; }

        [ForeignKey("orderId")]
        public int orderId { get; set; }

        public string name { get; set; }

        public int quantity { get; set; }

        // Refers to cost of 1 item, not many
        public double cost { get; set; }
    }
}
