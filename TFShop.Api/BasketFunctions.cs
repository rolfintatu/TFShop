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
        private readonly BasketItemRepository _itemRepo;
        private readonly ProductRepository _productRepo;
        private readonly BasketBuilder _basketBuilder;

        public BasketFunctions(BasketRepository basketRepo, 
            BasketItemRepository itemsRepo,
            ProductRepository productRepo,
            BasketBuilder basketBuilder)
        {
            _basketRepo = basketRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _itemRepo = itemsRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
            _basketBuilder = basketBuilder ?? throw new ArgumentNullException(nameof(BasketRepository));
        }

        //[FunctionName("CreateBasket")]
        //public async Task<IActionResult> CreateBasket(
        //    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    var basket = _basketBuilder.SimpleBasketCreation();
            
        //    await _basketRepo.InsertOrMerge(basket);

        //    return new OkObjectResult(basket.GetBasketIdAsString);
        //}

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
                var newBasketId = await CreateBasketWithItem(itemId);
                return new OkObjectResult(newBasketId);
            }
            else
            {
                await AddItemToBasket(itemId, basketId);
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

            await _itemRepo.GetBasketItems(basketId, out List<BasketItem> items);

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

            var basket = await _basketRepo.GetBasketAsync(basketId);

            if (basket is null)
                return new OkObjectResult(null);

            return new OkObjectResult(basket);
        }

        [FunctionName("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            var basketId = req.Form["basketId"].ToString();
            var itemId = req.Form["itemId"].ToString();
            var quantity = req.Form["newQuantity"].ToString();

            var basket = await _basketRepo.GetBasketAsync(basketId);

            if (int.Parse(quantity) == 0)
                basket.RemoveItem(Guid.Parse(itemId));
            else
                basket.UpdateQuantity(itemId, int.Parse(quantity));

            await _basketRepo.UpdateBasketWithItems(basket);

            return new OkResult();
        }

        //Private methods
        private async Task AddItemToBasket(string itemId, string basketId)
        {

            var isInBasket = await _itemRepo.IsInBasket(itemId, basketId);

            if (!isInBasket)
            {
                var product = await _productRepo.GetProductById(Guid.Parse(itemId));
                var basket = await _basketRepo.GetBasketAsync(basketId);
                basket.AddItem(itemId, product.Price, product.Name);
                await _basketRepo.UpdateBasketWithItems(basket);
            }
            else
            {
                var basket = await _basketRepo.GetBasketAsync(basketId);
                basket.UpdateQuantity(itemId);
                await _basketRepo.UpdateBasketWithItems(basket);
            }
        }
        private async Task<string> CreateBasketWithItem(string productId)
        {
            _basketBuilder.CreateBasket();
            var newBasket = _basketBuilder.GetBasket();
            var product = await _productRepo.GetProductById(Guid.Parse(productId));
            newBasket.AddItem(productId, product.Price, product.Name);
            await _basketRepo.UpdateBasketWithItems(newBasket);

            return newBasket.PartitionKey;
        }
    }


    public class UpdateQuantityModel
    {
        public string BasketId { get; set; }
        public string ItemId { get; set; }
        public string NewQuantity { get; set; }
    }
}
