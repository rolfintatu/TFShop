using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services.AggregateBasket
{
    public interface IBasketService
    {
        Task CreateBasket();
    }
}
