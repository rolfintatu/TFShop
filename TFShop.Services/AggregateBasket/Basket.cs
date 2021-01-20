using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TFShop.Services.Models;

namespace TFShop.Services.AggregateBasket
{
    public class Basket : TableEntity
    {
        public Basket()
        {
            BasketId = Guid.NewGuid();
            BasketOwner = Guid.Empty;
            _items = new List<BasketItem>();
        }

        [IgnoreProperty]
        public Guid BasketId {
            get { return Guid.Parse(PartitionKey); }
            set { PartitionKey = value.ToString(); }
        }

        [IgnoreProperty]
        public Guid BasketOwner
        {
            get { return Guid.Parse(RowKey); }
            set { RowKey = value.ToString(); }
        }

        public double Subtotal { get; set; }
        public double VAT { get; set; }
        public double Total { get; set; }

        private List<BasketItem> _items;

        [IgnoreProperty]
        public IReadOnlyList<BasketItem> Items
            => _items.AsReadOnly();

        public void SetItems(List<BasketItem> newItems)
        {
            _items = newItems;
        }

        public void AddItem(string itemId, double price, string name, int quantity = 1)
        {
            _ = _items ?? new();

            var basketItem = _items.Find(x => x.RowKey == itemId.ToString());

            if (basketItem is not null)
            {
                basketItem.IncreaseQuantity();
                CalculateSubtotal();
            }
            else
            {
                _items.Add(new BasketItem(Guid.Parse(this.PartitionKey), Guid.Parse(itemId), price, name));
                CalculateSubtotal();
            }
        }

        public void IncreaseItemQuantity(string itemId, int quantity = default(int))
        {
            if (!(quantity is default(int)))
            {
                _items.Find(x => x.RowKey == itemId).SetQuantityTo(quantity);
            }
            else
            {
                _items.Find(x => x.RowKey == itemId).IncreaseQuantity();
            }

            CalculateSubtotal();
        }

        public void RemoveItem(Guid itemId)
        {
            var item = _items.Find(x => x.RowKey == itemId.ToString());
            if (item is not null)
                _items.Remove(item);

            CalculateSubtotal();
        }

        public string GetBasketIdAsString
        {
            get { return this.BasketId.ToString(); }
        }

        // TODO: Improve this
        public BasketDetailsModel ToBasketDetails()
        {
            return new BasketDetailsModel(this.Subtotal, this.VAT, this.Total);
        }

        //Private methods
        private void CalculateSubtotal()
        {
            this.Subtotal = 0;
            _items.ToList().ForEach(x =>
            {
                this.Subtotal += (x.Price * x.Quantity);
            });
        }

    }
}
