﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrinkAndGo.Data.interfaces
{
    public class Product
    {
        public Product(int orderId, int orderDetailId, string name, decimal price, decimal orderTotal, string isSent, int amount)
        {
            OrderId = orderId;
            Id = orderDetailId;
            Name = name;
            Price = price;
            Total = orderTotal;
            Sent = isSent;
            Amount = amount;
        }

        public int OrderId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Sent { get; set; }
        public int Amount { get; set; }
    }

}
