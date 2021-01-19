using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services.AggregateBasket
{
    public interface BasketBuilder
    {
        void CreateBasket(Guid ownerId = default(Guid));
        void CreateBasketWithItems(ICollection<BasketItem> basketItems);
        Basket GetBasket();
        void SetBasket(Basket obj);
    }
}
