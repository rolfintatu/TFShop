using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services.AggregateBasket
{
    public class BasketRepository : BaseRepository
    {
        protected override string TABLE_NAME { get; set; } = "Basket";

        public async Task CreateBasket(Basket obj)
        {
            await _table.ExecuteAsync(TableOperation.InsertOrMerge(obj));
        }


    }
}
