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

        public async Task UpdateBasketWithItems(Basket obj)
        {
            if(obj is not null)
                await InsertOrMerge(obj);

            if(obj?.Items?.Count() != 0)
            {
                obj.Items.ToList().ForEach(async x =>
                {
                    if(x.IsModify)
                        await _itemsRepo.InsertOrMerge(x);
                });
            }
        }

        public Task<Basket> GetBasket(string basketId)
        {
            var basket = _table.CreateQuery<Basket>().Where(x => x.PartitionKey == basketId)
                .FirstOrDefault();

            if(basket is null)
                return Task.FromResult(basket);
            else
            {
                _itemsRepo.GetBasketItems(basketId, out List<BasketItem> items);
                basket.SetItems(items ?? new());
                return Task.FromResult(basket);
            }
        }

        public async Task<bool> BasketExist(string basketId)
        {
            var basket = await GetBasket(basketId);
            return basket is null ? false : true;
        }

    }
}
