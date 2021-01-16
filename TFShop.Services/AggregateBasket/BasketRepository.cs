using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace TFShop.Services.AggregateBasket
{
    public class BasketRepository : BaseRepository
    {
        protected override string TABLE_NAME { get; set; } = "Basket";
        private readonly BasketItemRepository _itemsRepo;

        public BasketRepository(BasketItemRepository itemsRepo)
        {
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
        }

        public async Task InsertOrMerge(Basket obj)
        {
            await _table.ExecuteAsync(TableOperation.InsertOrMerge(obj));
        }

        public Task<Basket> FetchBasket(string basketId)
        {
            return Task.FromResult(_table.CreateQuery<Basket>().Where(x => x.PartitionKey == basketId)
                .FirstOrDefault());
        }

        public async Task<bool> BasketExist(string basketId)
        {
            var basket = await FetchBasket(basketId);
            return basket is null ? false : true;
        }

    }
}
