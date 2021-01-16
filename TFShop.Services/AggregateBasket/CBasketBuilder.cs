using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class CBasketBuilder : BasketBuilder
    {
        private Basket _basket { get; set; }
        private ICollection<BasketItem> _basketItems { get; set; }

        public BasketBuilder CalculateTaxes(double price)
        {
            _basket.Subtotal += price;

            return this;
        }

        public BasketBuilder CreateBasket(Guid ownerId = default(Guid))
        {
            _basket ??= new Basket();
            _basket.PartitionKey = Guid.NewGuid().ToString();
            _basket.RowKey = ownerId.ToString();

            return this;
        }

        public BasketBuilder AdaptBasketItems(ICollection<BasketItem> basketItems)
        {
            if (basketItems != null)
                foreach (var item in basketItems)
                    item.BasketId = _basket.BasketId;

            _basketItems = basketItems;
            return this;
        }

        public BasketBuilder AttachAnItem(BasketItem item)
        {
            if (_basketItems is null) _basketItems = new List<BasketItem>();

            if (item is not null)
            {
                _basketItems.Add(item);
                _basket.Subtotal += (item.Price * item.Quantity);
            }

            return this;
        }

        public Basket GetBasket()
        {
            return _basket;
        }

        public void SetBasket(Basket obj)
        {
            this._basket = obj;
        }

        public BasketBuilder CalculateTaxes()
        {
            double subtotal = 0.0d;
            _basketItems.ToList().ForEach(x =>
            {
                subtotal += (x.Price * x.Quantity);
            });

            return this;
        }

        public ICollection<BasketItem> GetBasketItems()
        {
            return _basketItems;
        }
    }
}
