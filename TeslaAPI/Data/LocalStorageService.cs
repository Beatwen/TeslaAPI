using Microsoft.JSInterop;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace TeslaAPI.Data
{
    public interface ILocalStorageService
    {
        Task SetItemAsync<T>(string key, T value);
        Task<T?> GetItemAsync<T>(string key);
        Task RemoveItemAsync(string key);
    }

    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var serializedValue = JsonSerializer.Serialize(value);

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, serializedValue);
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            try
            {
                var serializedValue = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

                if (serializedValue == null)
                {
                    return default;
                }
                return JsonSerializer.Deserialize<T>(serializedValue);
            }
            catch (Exception ex)
            {
                Debug.Print($"Error retrieving item '{key}' from local storage: {ex.Message}");
                return default;
            }
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
    public class LocalStorageServiceWrapper : ILocalStorageService
    {
        private readonly LocalStorageService _localStorageService;

        public LocalStorageServiceWrapper(LocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            await _localStorageService.SetItemAsync(key, value);
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            return await _localStorageService.GetItemAsync<T>(key);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _localStorageService.RemoveItemAsync(key);
        }
    }

}
