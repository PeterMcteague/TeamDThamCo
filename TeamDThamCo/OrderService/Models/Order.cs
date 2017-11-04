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

        public int buyerId { get; set; }

        public string addressStreet { get; set; }

        public string addressCity { get; set; }

        public string addressPostCode { get; set; }

        public bool dispatched { get; set; }

        public bool invoiced { get; set; }

        public bool active { get; set; }
    }
}
