using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services
{
    public class Catalog
    {
        public Catalog() { Products = new List<Product>(); }

        public Catalog(Guid id, int version, DateTime date, List<Product> products)
        {
            Id = id;
            Version = version;
            Date = date;
            Products = products;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime Date { get; set; }
        public List<Product> Products { get; set; }

    }
}
