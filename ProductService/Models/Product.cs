using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models
{
    public class Product
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Ean { get; set; }
        public DateTime ExpectedRestock { get; set; }
        [Key, Column("Id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool InStock { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product() { }

    }
}
