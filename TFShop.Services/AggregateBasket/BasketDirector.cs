using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class BasketDirector
    {
        private BasketBuilder _basketBuilder;

        public BasketDirector(BasketBuilder basketBuilder)
        {
            _basketBuilder = basketBuilder ?? throw new ArgumentNullException(nameof(BasketDirector));
        }

        public void SetBuilder(BasketBuilder newBuilder)
        {
            _basketBuilder = newBuilder;
        }

        public Basket SimpleBasketCreation(Guid ownerId = default(Guid))
        {
            _basketBuilder.CreateBasket(ownerId);
            return _basketBuilder.GetBasket();
        }

        public Basket BasketWithTaxesCreation(double price)
        {
            return _basketBuilder.CreateBasket(Guid.Empty)
                .CalculateTaxes(price)
                .GetBasket();
        }

        public Basket UpdateBasketPrice(double price, Basket basket)
        {
            _basketBuilder.SetBasket(basket);
            return _basketBuilder.CalculateTaxes(price)
                .GetBasket();
        }

        public Basket BasketWithItemsCreation(ICollection<BasketItem> basketItems)
        {
            return _basketBuilder.CreateBasket()
                .AdaptBasketItems(basketItems)
                .CalculateTaxes()
                .GetBasket();
        }

        public ICollection<BasketItem> GetAttachedItems()
        {
            return _basketBuilder.GetBasketItems();
        }

        public Basket CreateBasketWithOneItem(BasketItem item)
        {
            return _basketBuilder.CreateBasket()
                .AttachAnItem(item)
                .GetBasket();
        }

    }
}
