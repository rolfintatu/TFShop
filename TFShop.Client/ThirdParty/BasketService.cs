using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFShop.Services.AggregateBasket;
using Blazored.LocalStorage;
using System.Net.Http;
using System.Text;

namespace TFShop.Client.ThirdParty
{
    public class BasketService : IBasketService
    {

        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public BasketService(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(BasketService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(BasketService));
        }


        /// <summary>
        /// Create a basket
        /// </summary>
        /// <returns></returns>
        public async Task CreateBasket()
        {
            var hasABasket = string.IsNullOrEmpty(await _localStorage.GetItemAsync<string>("_basket"))
                ? false
                : true;

            if (!hasABasket) {
                var response = await _httpClient.PostAsync("api/CreateBasket", new StringContent(""));

                if(response.IsSuccessStatusCode)
                await _localStorage.SetItemAsync<string>(
                    "_basket", await response.Content.ReadAsStringAsync()
                );
            }
        }
    }
}
