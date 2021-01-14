using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TFShop.Services.Models;

namespace TFShop.Services.AggregateBasket
{
    public interface IBasketService
    {
        Task CreateBasket();
        Task AddItemToBasket(Guid itemId);
        Task<List<BasketItemModel>> GetBasketItems();
    }
}
