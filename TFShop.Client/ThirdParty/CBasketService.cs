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
    public class CBasketService : BasketService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CBasketService(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(CBasketService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(CBasketService));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task AddItemToBasket(Guid itemId)
        {
            var basketId = await _localStorage.GetItemAsync<string>("_basket");

            var content = new Dictionary<string, string>()
            {
                { "BasketId", basketId },
                { "ItemId", itemId.ToString() }
            };

            var response = await _httpClient.PostAsync("api/AddItemToBasket", new FormUrlEncodedContent(content));

            if(response.IsSuccessStatusCode ^ response.Content.Headers.ContentLength == 0)
            {
                var contextStream = await response.Content.ReadAsStreamAsync();
                var newBasketId = await JsonSerializer.DeserializeAsync<string>(contextStream);
                await _localStorage.SetItemAsync("_basket", newBasketId);
            }
        }

        public async Task CreateBasket()
        {
            if (!await HasABasket()) {
                var response = await _httpClient.PostAsync("api/CreateBasket", new StringContent(""));
                if(response.IsSuccessStatusCode)
                    await _localStorage.SetItemAsync<string>(
                        "_basket", await response.Content.ReadAsStringAsync()
                    );
            }
        }

        public async Task<Basket> GetBasketDetails()
        {
            var basketId = await _localStorage.GetItemAsync<string>("_basket");
            var response = await _httpClient.GetAsync($"api/GetBasketDetails?basketId={basketId}");

            if (response.IsSuccessStatusCode ^ response.Content.Headers.ContentLength == 0)
            {
                var contextStream = await response.Content.ReadAsStreamAsync();
                var basket = await JsonSerializer.DeserializeAsync<Basket>(contextStream
                    , new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return basket;
            }

            return null;
        }

        public async Task<List<BasketItemModel>> GetBasketItems()
        {
            var basketId = await _localStorage.GetItemAsync<string>("_basket");

            if (string.IsNullOrWhiteSpace(basketId))
                return null;

            var response = await _httpClient.GetAsync($"/api/GetCartItems?basketId={basketId}");

            if (response.IsSuccessStatusCode)
            {
                var items = JsonSerializer.Deserialize<List<BasketItemModel>>(
                        await response.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                    );

                return items;
            }

            return null;
        }


        private async Task<bool> HasABasket()
        {
            var hasABasket = string.IsNullOrEmpty(
                await _localStorage.GetItemAsStringAsync("_basket"))
                ? false
                : true;

            return hasABasket;
        }
    }
}
