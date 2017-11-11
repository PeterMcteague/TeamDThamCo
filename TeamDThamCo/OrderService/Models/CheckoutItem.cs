using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class CheckoutItem
    {
        int id { get; set; }
        IEnumerable<Order> orders { get; set; }
    }
}
