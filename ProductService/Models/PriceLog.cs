using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models
{
    public class PriceLog
    {
        [Key, Column("Id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int productId { get; set; }
        public decimal OldPrice { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
