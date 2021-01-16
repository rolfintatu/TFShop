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
        private readonly BasketBuilder _basketBuilder;

        public BasketFunctions(BasketRepository basketRepo, 
            BasketItemRepository itemsRepo,
            ProductRepository productRepo,
            BasketBuilder basketBuilder)
        {
            _basketRepo = basketRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _basketBuilder = basketBuilder ?? throw new ArgumentNullException(nameof(BasketRepository));
        }

        [FunctionName("CreateBasket")]
        public async Task<IActionResult> CreateBasket(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var basket = _basketBuilder.CreateBasket().GetBasket();
            
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

            var isInBasket = await _itemsRepo.IsInBasket(itemId, basketId);

            if (!string.IsNullOrEmpty(basketId))
            {
                if (!isInBasket)
                {
                    var product = await _productRepo.GetProductById(Guid.Parse(itemId));
                    var basketItem = product.Zip(basketId);
                    await BasketPricing(basketId, product.Price);
                    await _itemsRepo.InsertOrMerge(basketItem);
                    await _basketRepo.InsertOrMerge(_basketBuilder.GetBasket());
                    return new OkResult();
                }
                else
                {
                    var basketItem = await _itemsRepo.GetItemFromBasketAsync(basketId, itemId);
                    basketItem.IncreaseQuantity();
                    await BasketPricing(basketId, basketItem.Price);
                    await _itemsRepo.InsertOrMerge(basketItem);
                    await _basketRepo.InsertOrMerge(_basketBuilder.GetBasket());
                    return new OkResult();
                }
            }

            return new BadRequestResult();
        }

        [FunctionName("GetCartItems")]
        public async Task<IActionResult> GetBasketItems(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            var basketId = req.Query["basketId"].ToString();

            if (string.IsNullOrWhiteSpace(basketId))
                return new OkResult();

            var items = await _itemsRepo.GetBasketItems(basketId);

            if(items.Count != 0)
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

            return new OkObjectResult(basket);
        }


        private async Task BasketPricing(string basketId, double productPrice)
        {
            var basket = await _basketRepo.FetchBasket(basketId);
            _basketBuilder.SetBasket(basket);
            _basketBuilder.Calculate(productPrice);
        }
    }
}
