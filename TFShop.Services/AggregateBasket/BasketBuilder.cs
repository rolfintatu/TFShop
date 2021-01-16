using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TFShop.Services.AggregateBasket
{
    public interface BasketBuilder
    {
        BasketBuilder CreateBasket(Guid ownerId = default(Guid));
        BasketBuilder Calculate(double price);
        Basket GetBasket();
        void SetBasket(Basket obj);
    }
}
