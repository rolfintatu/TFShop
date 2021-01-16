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

        public async Task InsertOrMergeRange(ICollection<BasketItem> items)
        {
            List<Task> tasks = new List<Task>();
            foreach (var item in items)
            {
                tasks.Add(InsertOrMerge(item));
            }
            await Task.WhenAll(tasks);
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

        public Task<bool> GetBasketItems(string basketId, out List<BasketItem> basketItems)
        {
            var items = _table.CreateQuery<BasketItem>()
                .Where(y => y.PartitionKey == basketId.ToString());

            if (items.ToList().Any())
            {
                basketItems = items.ToList();
                return Task.FromResult(true);
            }
            else
            {
                basketItems = null;
                return Task.FromResult(false);
            }
        }
    }
}
