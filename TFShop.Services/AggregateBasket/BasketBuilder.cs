using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services.AggregateBasket
{
    public interface BasketBuilder
    {
        BasketBuilder CreateBasket(Guid ownerId = default(Guid));
        BasketBuilder CalculateTaxes(double price);
        BasketBuilder CalculateTaxes();
        BasketBuilder AdaptBasketItems(ICollection<BasketItem> basketItems);
        Basket GetBasket();
        void SetBasket(Basket obj);
        BasketBuilder AttachAnItem(BasketItem item);
        ICollection<BasketItem> GetBasketItems();
    }
}
