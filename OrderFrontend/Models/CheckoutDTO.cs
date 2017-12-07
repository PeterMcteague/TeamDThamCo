using OrderFrontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFrontend.Models
{
    public class CheckoutDTO
    {
        public string buyerId { get; set; }
        public string buyerAddress { get; set; }
        public bool canPurchase { get; set; }

        public List<OrderItem> items { get; set; }
        public decimal totalCost { get; set; }

        public string stripeKey { get; set; }
        public string continueUrl { get; set; }
    }
}
