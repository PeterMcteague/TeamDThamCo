using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFrontend.Models
{
    public class HistoryDTO
    {
        public List<OrderAndItemDTO> orderAndItems { get; set; }
        public bool canPurchase { get; set; }
        public decimal total { get; set; }
    }
}
