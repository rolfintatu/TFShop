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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
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

            //var itemid = req.Form["itemid"].ToString();

            var result = await JsonSerializer.DeserializeAsync<HttpRes>(req.Body);

            var product = await _productRepo.GetProductById(Guid.Parse(result.itemId));

            var quantityIfExixt = await _itemsRepo.GetQuantityIfExist(product.Id, Guid.Parse(result.basketId));

            if (result.basketId != null)
            {
                if (quantityIfExixt == 0)
                {
                    await _itemsRepo.AddItemToBasketAsync(
                        new BasketItem(
                            Guid.Parse(result.basketId),
                            Guid.Parse(result.itemId),
                            1,
                            product.Price)
                        );
                    return new OkResult();
                }
                else
                {
                    await _itemsRepo.AddItemToBasketAsync(
                        new BasketItem(
                            Guid.Parse(result.basketId),
                            product.Id,
                            quantityIfExixt += 1,
                            product.Price)
                        );
                    return new OkResult();
                }
            }
            else
                return new BadRequestResult();
        }

        internal class HttpRes
        {
            public string itemId { get; set; }
            public string basketId { get; set; }
        }
    }
}
