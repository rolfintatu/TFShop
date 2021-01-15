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

        public BasketFunctions(BasketRepository basketRepo, 
            BasketItemRepository itemsRepo,
            ProductRepository productRepo)
        {
            _basketRepo = basketRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
        }

        [FunctionName("CreateBasket")]
        public async Task<IActionResult> CreateBasket(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Basket basket = new Basket();
            
            await _basketRepo.CreateBasket(basket);

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
                    await _itemsRepo.InsertOrMerge(
                        product.Zip(basketId));
                    return new OkResult();
                }
                else
                {
                    var basketItem = await _itemsRepo.GetItemFromBasketAsync(basketId, itemId);
                    basketItem.IncreaseQuantity();
                    await _itemsRepo.InsertOrMerge(basketItem);
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

            return items.Count != 0 
                ? new OkObjectResult(items) 
                : new NotFoundResult();
        }
    }
}
