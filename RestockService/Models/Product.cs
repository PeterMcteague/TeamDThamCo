using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RestockService.Models
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
        public int Id { get; set; }
        public bool InStock { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int SupplierId { get; set; }
        public int SupplierProductId { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime When { get; set; }
        public string ProductName { get; set; }
        public string ProductEan { get; set; }
        public decimal TotalPrice { get; set; }

        public Product() { }

    }


}
