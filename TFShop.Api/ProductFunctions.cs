using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TFShop.Services;

namespace TFShop.Api
{
    public class ProductFunctions
    {

        private readonly ProductRepository _prodRepo;

        public ProductFunctions(ProductRepository prodRepo)
        {
            _prodRepo = prodRepo ?? throw new ArgumentNullException(nameof(ProductFunctions));
        }

        [FunctionName("GetProducts")]
        public async Task<IActionResult> GetProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var products = await _prodRepo.GetProducts();

            return new OkObjectResult(products);
        }
    }
}
