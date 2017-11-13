using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerDetail.Models
{
    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
