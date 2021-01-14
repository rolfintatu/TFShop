using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFShop.Services.AggregateBasket;
using Blazored.LocalStorage;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using TFShop.Services.Models;

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
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task AddItemToBasket(Guid itemId)
        {
            var basketId = await _localStorage.GetItemAsync<string>("_basket");

            if (string.IsNullOrEmpty(basketId))
                return;

            var content = new Dictionary<string, string>()
            {
                { "BasketId", basketId },
                { "ItemId", itemId.ToString() }
            };

            var response = await _httpClient.PostAsync("api/AddItemToBasket", new FormUrlEncodedContent(content));
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

        public async Task<List<BasketItemModel>> GetBasketItems()
        {
            var basketId = await _localStorage.GetItemAsync<string>("_basket");

            if (string.IsNullOrWhiteSpace(basketId))
                return null;

            var result = await _httpClient.GetAsync($"/api/GetCartItems?basketId={basketId}");

            if (result.IsSuccessStatusCode)
            {
                var items = JsonSerializer.Deserialize<List<BasketItemModel>>(
                        await result.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                    );

                return items;
            }

            return null;
        }

    }

    internal class HttpRes
    {
        public string itemId { get; set; }
        public string basketId { get; set; }
    }
}
