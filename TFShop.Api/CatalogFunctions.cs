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
    public class CatalogFunctions
    {

        private readonly FetchData _fatch;
        private readonly ProductRepository _repo;

        public CatalogFunctions(FetchData fatch, ProductRepository repo)
        {
            _fatch = fatch;
            _repo = repo;
        }

        [FunctionName("GetData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            Catalog catalog = await _fatch.GetValues();

            return new OkObjectResult(catalog);
        }

        [FunctionName("SeedData")]
        public async Task<IActionResult> SeedData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _repo.SeedProducts();

            return new OkResult();
        }
    }
}
