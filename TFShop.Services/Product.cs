using Microsoft.Azure.Cosmos.Table;
using System;
using TFShop.Services.AggregateBasket;

namespace TFShop.Services
{
    public class Product : TableEntity
    {
        public Product()
        {
        }

        public Product(Guid id, string name, double price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public Guid Id {
            get { return Guid.Parse(this.PartitionKey); }
            set { this.PartitionKey = value.ToString(); } 
        }

        public string Name {
            get { return this.RowKey; }
            set { this.RowKey = value; } }

        public double Price { get; set; }

        public BasketItem Zip(string basketId)
        {
            return new BasketItem
            {
                PartitionKey = basketId,
                RowKey = this.PartitionKey,
                Price = this.Price,
                Name = this.Name
            };
        }

        public BasketItem Zip()
        {
            return new BasketItem
            {
                RowKey = this.PartitionKey,
                Price = this.Price,
                Name = this.Name
            };
        }
    }
}