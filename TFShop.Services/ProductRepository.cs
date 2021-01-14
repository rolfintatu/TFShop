using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services
{
    public class ProductRepository : BaseRepository
    {
        protected override string TABLE_NAME { get; set; } = "Products";

        public async Task SeedProducts()
        {
            List<Product> products = new List<Product>()
            {
                new Product(Guid.Parse("63ccb9e8-0d50-4abd-a88f-0240fa3fdd16"), "Product1", 32),
                new Product(Guid.Parse("067de05d-3156-4704-bf58-18f56f3fef74"), "Product2", 52),
                new Product(Guid.Parse("4d0052fa-97e4-47b9-9d0b-1ced2a976ae7"), "Product3", 45),
                new Product(Guid.Parse("16439329-d876-4a6a-bb59-04972e0679a5"), "Product4", 21)
            };

            foreach (var product in products)
            {
                await _table.ExecuteAsync(TableOperation.InsertOrReplace(product));
            }
        }

        public Task<Product> GetProductById(Guid productId)
        {

            var result = _table.CreateQuery<Product>()
                .Where(x => x.Id == productId);

            return Task.FromResult(result.FirstOrDefault());
        }

        public Task<List<Product>> GetProducts()
        {
            return Task.FromResult( _table.CreateQuery<Product>().ToList());
        }
    }
}
