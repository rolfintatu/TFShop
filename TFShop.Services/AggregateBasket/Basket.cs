using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class Basket : TableEntity
    {
        public Basket()
        {
            BasketId = Guid.NewGuid();
            BasketOwner = Guid.Empty;
        }

        [IgnoreProperty]
        public Guid BasketId {
            get { return Guid.Parse(PartitionKey); }
            set { PartitionKey = value.ToString(); }
        }

        [IgnoreProperty]
        public Guid BasketOwner
        {
            get { return Guid.Parse(RowKey); }
            set { RowKey = value.ToString(); }
        }

        public double Subtotal { get; set; }
        public double VAT { get; set; }
        public double Total { get; set; }

        public string GetBasketIdAsString
        {
            get { return this.BasketId.ToString(); }
        }
    }
}
