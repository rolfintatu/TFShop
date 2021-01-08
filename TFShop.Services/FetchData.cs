using System;
using System.Threading.Tasks;

namespace TFShop.Services
{
    public class FetchData
    {

        public Task<Catalog> GetValues()
        {
            return Task.FromResult(new Catalog(
                Guid.NewGuid(), 1, DateTime.UtcNow,
                new System.Collections.Generic.List<Product>()
                {
                    new Product(Guid.NewGuid(), "Product1", 32),
                    new Product(Guid.NewGuid(), "Product2", 52),
                    new Product(Guid.NewGuid(), "Product3", 45),
                    new Product(Guid.NewGuid(), "Product4", 21),
                }));
        }

    }
}
