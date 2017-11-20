using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public DateTime orderDate { get; set; }
        
        [ForeignKey("buyerId")]
        public string buyerId { get; set; }

        public string address { get; set; }

        public bool dispatched { get; set; }

        public bool invoiced { get; set; }

        public bool active { get; set; }
    }
}
