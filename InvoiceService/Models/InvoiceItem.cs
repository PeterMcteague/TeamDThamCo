using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceService.Models
{
    public class InvoiceItem
    {
        public string customerId { get; set; } //The customer that the invoice belongs to
        public List<int> orderIds { get; set; } //The orderId's this invoice refers to
    }
}
