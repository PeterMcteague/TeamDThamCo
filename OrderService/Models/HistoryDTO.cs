using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class HistoryDTO
    {
        public List<OrderAndItemDTO> orderAndItems { get; set; }
        public bool canPurchase { get; set; }
    }
}
