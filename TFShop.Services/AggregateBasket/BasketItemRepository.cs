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

        public Task<List<BasketItem>> GetItemsByBasketAsync(Guid basketId)
        {
            var result = _table.CreateQuery<BasketItem>().Where(x => x.BasketId == basketId);

            if (result != null)
                return Task.FromResult(result.ToList());
            else
                return null;
        }

        public async Task AddItemToBasketAsync(BasketItem item)
        {
            await _table.ExecuteAsync(TableOperation.InsertOrMerge(item));
        }

        public Task<int> GetQuantityIfExist(Guid itemId, Guid basketId)
        {
            var basketItems = _table.CreateQuery<BasketItem>()
                .Where(x => x.RowKey == itemId.ToString() && x.PartitionKey == basketId.ToString()).ToList();

            return Task.FromResult(basketItems.Count != 0 ? basketItems.First().Quantity : 0);
        }
    }
}
