﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class OrderAndItemDTO
    {
        public OrderAndItemDTO(Order orders, List<OrderItem> items)
        {
            Order = orders;
            Items = items;
        }

        public Order Order { get; set; }
        public List<OrderItem> Items {get;set;}
    }
}
