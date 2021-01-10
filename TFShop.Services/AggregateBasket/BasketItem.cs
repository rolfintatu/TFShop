using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class BasketItem : TableEntity
    {

        [IgnoreProperty]
        public Guid BasketId {
            get { return Guid.Parse(PartitionKey); }
            set { PartitionKey = value.ToString(); } 
        }

        [IgnoreProperty]
        public Guid ProductId {
            get { return Guid.Parse(RowKey); } 
            set { PartitionKey = value.ToString(); } 
        }

        public int Quantity { get; set; }
        public double Price { get; set; }

    }
}
