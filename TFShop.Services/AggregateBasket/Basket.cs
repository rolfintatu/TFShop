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
            protected set { PartitionKey = value.ToString(); }
        }

        [IgnoreProperty]
        public Guid BasketOwner
        {
            get { return Guid.Parse(RowKey); }
            protected set { RowKey = value.ToString(); }
        }

        public string GetBasketIdAsString
        {
            get { return this.BasketId.ToString(); }
        }
    }
}
