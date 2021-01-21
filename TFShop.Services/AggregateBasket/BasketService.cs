using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TFShop.Services.Models;

namespace TFShop.Services.AggregateBasket
{
    public interface BasketService
    {
        /// <summary>
        /// Create a basket if don't exist.
        /// </summary>
        /// <returns></returns>
        //Task CreateBasket();

        /// <summary>
        /// Add an item/product in basket. If already exist into car will increase
        /// quantity for it.
        /// </summary>
        /// <param name="itemId">ID of a product/item you want to add into basket.</param>
        /// <returns></returns>
        Task AddItemToBasket(Guid itemId);

        /// <summary>
        /// Return a list with items for a specific basket.
        /// </summary>
        /// <returns>A list of items or null if the basket doesn't have items.</returns>
        Task<List<BasketItemModel>> GetBasketItems();

        /// <summary>
        /// Basket details like subtotal, VAT and total
        /// </summary>
        /// <returns>A basket with all necessary data</returns>
        Task<Basket> GetBasketDetails();

        Task UpdateQuantityFor(string itemId, int quantity);
    }
}
