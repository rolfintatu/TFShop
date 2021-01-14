using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class BasketItem : TableEntity
    {
        public BasketItem() { }

        public BasketItem(Guid basketId, Guid productId, int quantity, double price, string name)
        {
            BasketId = basketId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            Name = name;
        }

        [IgnoreProperty]
        public Guid BasketId {
            get { return Guid.Parse(PartitionKey); }
            set { PartitionKey = value.ToString(); } 
        }

        [IgnoreProperty]
        public Guid ProductId {
            get { return Guid.Parse(RowKey); } 
            set { RowKey = value.ToString(); } 
        }

        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

    }
}
