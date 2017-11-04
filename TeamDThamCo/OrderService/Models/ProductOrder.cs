using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    //Relational model between products and orders
    public class ProductOrder
    {
        public int id { get; set; }

        public int orderId { get; set; }

        public int productId { get; set; }

        public int amountOrdered { get; set; }
    }
}
