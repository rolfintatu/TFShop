using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using TFShop.Services.Enums;

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
            ItemStatus = BasketItemStatus.Added;
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
        public BasketItemStatus ItemStatus { get; set; }


        public string Name { get; set; }
        public int Quantity { get; set; } = 1;
        public double Price { get; set; }

        public void IncreaseQuantity()
        {
            Quantity += 1;
            ItemStatus = BasketItemStatus.Modified;
        }

        public void SetQuantityTo(int quantity)
        {
            Quantity = quantity;
            ItemStatus = BasketItemStatus.Modified;
        }
    }
}
