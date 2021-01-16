using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TFShop.Services.AggregateBasket;
using TFShop.Services;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace TFShop.Api
{
    public class BasketFunctions
    {
        private readonly BasketRepository _basketRepo;
        private readonly BasketItemRepository _itemsRepo;
        private readonly ProductRepository _productRepo;
        private readonly BasketDirector _basketDirector;

        public BasketFunctions(BasketRepository basketRepo, 
            BasketItemRepository itemsRepo,
            ProductRepository productRepo,
            BasketDirector basketDirector)
        {
            _basketRepo = basketRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _basketDirector = basketDirector ?? throw new ArgumentNullException(nameof(BasketRepository));
        }

        [FunctionName("CreateBasket")]
        public async Task<IActionResult> CreateBasket(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var basket = _basketDirector.SimpleBasketCreation();
            
            await _basketRepo.InsertOrMerge(basket);

            return new OkObjectResult(basket.GetBasketIdAsString);
        }

        [FunctionName("AddItemToBasket")]
        public async Task<IActionResult> AddItemToBasket(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var itemId = req.Form["itemid"].ToString();
            var basketId = req.Form["basketId"].ToString();

            if (string.IsNullOrEmpty(basketId))
            {
                var newBasketId = await CreateBasketWithItem(itemId);
                return new OkObjectResult(newBasketId);

            } else if (!string.IsNullOrEmpty(basketId) && !await _basketRepo.BasketExist(basketId))
            {
                //if (await _itemsRepo.GetBasketItems(basketId, out List<BasketItem> items))
                //{
                //    var newBasket = _basketDirector.BasketWithItemsCreation(items);
                //    await _basketRepo.InsertOrMerge(newBasket);
                //    await _itemsRepo.InsertOrMergeRange(_basketDirector.GetAttachedItems());
                //    return new OkObjectResult(newBasket.GetBasketIdAsString);
                //}

                var newBasketId = await CreateBasketWithItem(itemId);
                return new OkObjectResult(newBasketId);
            }
            else
            {
                await AddIfCartExist(itemId, basketId);
                return new OkResult();
            }
        }

        [FunctionName("GetCartItems")]
        public async Task<IActionResult> GetBasketItems(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            var basketId = req.Query["basketId"].ToString();

            if (string.IsNullOrWhiteSpace(basketId))
                return new NotFoundResult();

            await _itemsRepo.GetBasketItems(basketId, out List<BasketItem> items);

            if(items is not null && await _basketRepo.BasketExist(basketId))
                return new OkObjectResult(items);
            else
                return new NotFoundResult();
        }

        [FunctionName("GetBasketDetails")]
        public async Task<IActionResult> GetBasketDetails(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            string basketId = req.Query["basketId"].ToString();

            if (string.IsNullOrWhiteSpace(basketId))
                return new BadRequestResult();

            var basket = await _basketRepo.FetchBasket(basketId);

            if (basket is null)
                return new OkObjectResult(null);

            return new OkObjectResult(basket);
        }


        //Private methods
        private async Task<Basket> BasketPricing(string basketId, double productPrice)
        {
            var basket = await _basketRepo.FetchBasket(basketId);
            return _basketDirector.UpdateBasketPrice(productPrice, basket);
        }
        private async Task AddIfCartExist(string itemId, string basketId)
        {
            var isInBasket = await _itemsRepo.IsInBasket(itemId, basketId);

            if (!isInBasket)
            {
                var product = await _productRepo.GetProductById(Guid.Parse(itemId));
                var basket = _basketDirector.UpdateBasketPrice(product.Price, await _basketRepo.FetchBasket(basketId));
                await _basketRepo.InsertOrMerge(basket);
                var basketItem = product.Zip(basketId);
                await _itemsRepo.InsertOrMerge(basketItem);
            }
            else
            {
                var basketItem = await _itemsRepo.GetItemFromBasketAsync(basketId, itemId);
                basketItem.IncreaseQuantity();
                var basket = await BasketPricing(basketId, basketItem.Price);
                await _itemsRepo.InsertOrMerge(basketItem);
                await _basketRepo.InsertOrMerge(basket);
            }
        }
        private async Task<string> CreateBasketWithItem(string itemId)
        {
            var item = await _productRepo.GetProductById(Guid.Parse(itemId));
            var newBasket = _basketDirector.CreateBasketWithOneItem(item.Zip());
            await _basketRepo.InsertOrMerge(newBasket);
            await _itemsRepo.InsertOrMerge(item.Zip(newBasket.PartitionKey));

            return newBasket.PartitionKey;
        }
    }
}
