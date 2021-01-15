using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services.AggregateBasket
{
    public class BasketItemRepository : BaseRepository
    {
        protected override string TABLE_NAME { get; set; } = "BasketItems";

        public Task<BasketItem> GetItemFromBasketAsync(string basketId, string itemId)
        {
            var item = _table.CreateQuery<BasketItem>()
                .Where(x => x.PartitionKey == basketId && x.RowKey == itemId)
                .FirstOrDefault();

            return item != null 
                ? Task.FromResult(item)
                : null;
        }

        public async Task InsertOrMerge(BasketItem item)
        {
            await _table.ExecuteAsync(TableOperation.InsertOrMerge(item));
        }

        public Task<bool> IsInBasket(string itemId, string basketId)
        {
            var isInBasket = _table.CreateQuery<BasketItem>()
                    .Where(x => x.RowKey == itemId && x.PartitionKey == basketId)
                    .FirstOrDefault();

            return Task.FromResult(isInBasket == null ? false : true);
        }

        public Task<List<BasketItem>> GetBasketItems(string basketId)
        {
            var items = _table.CreateQuery<BasketItem>()
                .Where(y => y.PartitionKey == basketId.ToString());

            return Task.FromResult(items.ToList());
        }
    }
}
