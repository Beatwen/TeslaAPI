namespace TeslaAPI.Component
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
    using TeslaAPI.Data;
    using Microsoft.JSInterop;

    public class PartnerToken
    {
        string token { get; set; }
        public static async Task<string> GetPartnerToken(IJSRuntime jsRuntime)
        {
            Debug.Print("GetPartnerToken");
            LocalStorageService _storageService = new LocalStorageService(jsRuntime);
            string token = await _storageService.GetItemAsync<string>("PartnerToken");
            bool valid = await IsTokenStillValid(_storageService);
            if (token != null && valid)
            {
                Debug.Print("Found token in local storage: " + token);
                Debug.Print("Token is still valid.");
                return token;
            }
            else
            {
                Debug.Print("Token not found in local storage. Generating a new one...");
                return await GeneratePartnerToken(_storageService);
            }

        }
        static async Task<bool> IsTokenStillValid(LocalStorageService _localStorageService)
        {
            var expiresAt = await _localStorageService.GetItemAsync<DateTime>("PartnerTokenExpiresAt");
            return expiresAt > DateTime.Now;
        }
        static async Task<string> GeneratePartnerToken(LocalStorageService _localStorageService)
        {
            Debug.Print("Generating partner access token...");
            string clientId = "285b750b0c21-49a2-a9af-c44c1f100566";
            string clientSecret = "ta-secret.rTLXH7fIyZwU5PTf";
            using (var httpClient = new HttpClient())
            {
                var tokenRequest = new
                {
                    grant_type = "client_credentials",
                    client_id = clientId,
                    client_secret = clientSecret,
                    scope = ""
                };
                var tokenRequestJson = JsonSerializer.Serialize(tokenRequest);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.tesla.com/oauth2/v3/token")
                {
                    Content = new StringContent(tokenRequestJson, Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    string partnerAccessToken = tokenResponse.access_token;
                    Debug.Print("Partner Access Token: " + partnerAccessToken + " valid until: " + DateTime.Now.AddSeconds(tokenResponse.expires_in));

                    // Store the new token and its expiration time in local storage
                    await _localStorageService.SetItemAsync("PartnerToken", partnerAccessToken);
                    await _localStorageService.SetItemAsync("PartnerTokenExpiresAt", DateTime.Now.AddSeconds(tokenResponse.expires_in));
                    return partnerAccessToken;
                }
                else
                {
                    Debug.Print("Failed to obtain partner access token. Status code: " + response.StatusCode);
                    return null;
                }
            }
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }
    public class ErrorResponse
    {
        public string error_code { get; set; }
        public string error_description { get; set; }
        // Add more properties if needed based on the API's error response structure
    }

}
