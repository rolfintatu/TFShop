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

        public CatalogFunctions(FetchData fatch)
        {
            _fatch = fatch;
        }

        [FunctionName("GetData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            Catalog catalog = await _fatch.GetValues();

            return new OkObjectResult(catalog);
        }
    }
}
