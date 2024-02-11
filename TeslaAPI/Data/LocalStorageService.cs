using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text.Json;

namespace TeslaAPI.Data
{
    public class LocalStorageService
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

        public async Task<T> GetItemAsync<T>(string key)
        {
            try
            {
                var serializedValue = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

                // Check if the serialized value is null
                if (serializedValue == null)
                {
                    // Item does not exist in local storage, return null
                    return default;
                }

                // Deserialize the value
                return JsonSerializer.Deserialize<T>(serializedValue);
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., log the error)
                Debug.Print($"Error retrieving item '{key}' from local storage: {ex.Message}");
                return default;
            }
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
