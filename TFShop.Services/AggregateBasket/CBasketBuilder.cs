using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.AggregateBasket
{
    public class CBasketBuilder : BasketBuilder
    {
        private Basket _basket { get; set; }

        public BasketBuilder Calculate(double price)
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
