using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class CBasketBuilder : BasketBuilder
    {
        private Basket _basket { get; set; }

        public void CreateBasket(Guid ownerId = default(Guid))
        {
            _basket ??= new Basket();
            _basket.PartitionKey = Guid.NewGuid().ToString();
            _basket.RowKey = ownerId.ToString();
        }

        public void CreateBasketWithItems(ICollection<BasketItem> basketItems)
        {
            if (basketItems != null)
                foreach (var item in basketItems)
                    item.BasketId = _basket.BasketId;
        }

        public Basket GetBasket()
        {
            return _basket;
        }

        public void SetBasket(Basket obj)
        {
            this._basket = obj;
        }
    }
}
