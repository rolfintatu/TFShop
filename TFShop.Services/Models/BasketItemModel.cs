using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.Models
{
    public class BasketItemModel
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
