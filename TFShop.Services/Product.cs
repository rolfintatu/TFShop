using Microsoft.Azure.Cosmos.Table;
using System;

namespace TFShop.Services
{
    public class Product : TableEntity
    {
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
    }
}