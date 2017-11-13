using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BasketService.Models
{
    public class BasketItem
    {
        public int id { get; set; }

        [ForeignKey("buyerId")]
        public string buyerId { get; set; }

        [ForeignKey("productId")]
        public int productId { get; set; }

        public int quantity { get; set; }
    }
}
