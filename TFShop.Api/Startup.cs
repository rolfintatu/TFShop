using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TFShop.Services;

[assembly: FunctionsStartup(typeof(TFShop.Api.Startup))]

namespace TFShop.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<FetchData>();
            builder.Services.AddScoped<ProductRepository>();

        }
    }
}
