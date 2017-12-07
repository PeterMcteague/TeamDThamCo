using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class BasketItem
    {
        public decimal cost { get; set; }

        public int quantity { get; set; }
    }
}
