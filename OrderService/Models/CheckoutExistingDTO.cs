using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class CheckoutExistingDTO : CheckoutDTO
    {
        public int OrderId {get;set;}
    }
}
