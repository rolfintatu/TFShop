using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TFShop.Services.AggregateBasket;

namespace TFShop.Api
{
    public class BasketFunctions
    {
        private readonly BasketRepository _basketRepo;

        public BasketFunctions(BasketRepository basketRepo)
        {
            _basketRepo = basketRepo ?? throw new ArgumentNullException(nameof(BasketRepository));
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
    }
}
