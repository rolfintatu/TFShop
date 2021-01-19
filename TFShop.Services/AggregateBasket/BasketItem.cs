using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class BasketItem : TableEntity
    {
        public BasketItem() { }

        public BasketItem(Guid basketId, Guid productId, double price, string name)
        {
            BasketId = basketId;
            ProductId = productId;
            Price = price;
            Name = name;
            IsModify = true;
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

        [IgnoreProperty]
        public bool IsModify { get; set; } = false;

        public string Name { get; set; }
        public int Quantity { get; set; } = 1;
        public double Price { get; set; }

        public void IncreaseQuantity()
        {
            Quantity += 1;
            IsModify = true;
        }

        public void SetQuantityTo(int quantity)
        {
            Quantity = quantity;
            IsModify = true;
        }
    }
}
