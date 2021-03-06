using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TFShop.Services;
using Blazored.LocalStorage;
using TFShop.Services.AggregateBasket;
using TFShop.Client.ThirdParty;

namespace TFShop.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var BaseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(BaseAddress) });

            builder.Services.AddBlazoredLocalStorage(x => x.JsonSerializerOptions.WriteIndented = true);
            builder.Services.AddTransient<BasketService, CBasketService>();

            await builder.Build().RunAsync();
        }
    }
}
