using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order
    {
        public int id { get; set; }

        public DateTime orderDate { get; set; }

        public ICollection<OrderItem> products {get;set;}

        public string buyerId { get; set; }

        public string address { get; set; }

        public bool dispatched { get; set; }

        public bool invoiced { get; set; }

        public bool active { get; set; }
    }
}
