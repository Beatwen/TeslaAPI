using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text.Json;
using TeslaAPI.Data;
using static System.Net.WebRequestMethods;

namespace TeslaAPI.Component
{
    public class UserToken
    {
        static string baseUrl = "https://59dd-2a02-2788-11c4-afd-82f-3a5f-a945-625f.ngrok-free.app";
        static string redirectUrl = $"{baseUrl}/oauth-callback";
        
        public static async Task<string> GetUserToken(NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            LocalStorageService localStorageService = new LocalStorageService(jsRuntime);
            string token = await localStorageService.GetItemAsync<string>("UserToken");
            Debug.Print(token);
            if (token != null && await IsTokenStillValid(localStorageService)) 
            {
                Debug.Print("Found Token in Local Storage and is valid" + token);
                return token;
            }
            else
            {
                Debug.Print("No Token Found in Local Storage and is valid" + token);
                RedirectUser(navigationManager, jsRuntime);
                return null;
            }

        }
        static async Task<bool> IsTokenStillValid(LocalStorageService localStorageService)
        {
            var expiresAt = await localStorageService.GetItemAsync<DateTime>("UserTokenExpiresAt");
            return expiresAt > DateTime.Now;
        }
        static void RedirectUser(NavigationManager navigationManager, IJSRuntime jSRuntime)
        {
            
            string codeVerifier = Code.GenerateRandomString(86);
            string state = Code.GenerateRandomString(32);
            string authorizeUrl = $"https://auth.tesla.com/oauth2/v3/authorize?&client_id=285b750b0c21-49a2-a9af-c44c1f100566&locale=en-US&prompt=login&redirect_uri={redirectUrl}&response_type=code&scope=openid%20offline_access%20user_data%20vehicle_device_data%20vehicle_cmds%20vehicle_charging_cmds&state={state}";
            using (var client = new HttpClient())
            {
                navigationManager.NavigateTo(authorizeUrl);
            }
        }
        public static void RedirectUserToIndex(NavigationManager navigationManager, IJSRuntime jSRuntime)
        {
            using (var client = new HttpClient())
            {
                navigationManager.NavigateTo(baseUrl);
            }
        }
        public static async Task<string> GenerateUserToken(string authorizationCode, IJSRuntime jsRuntime)
        {
                // Replace these placeholders with your actual values
                string tokenEndpoint = "https://auth.tesla.com/oauth2/v3/token";
                string clientId = "285b750b0c21-49a2-a9af-c44c1f100566";
                string clientSecret = "ta-secret.rTLXH7fIyZwU5PTf";
                string scope = "openid offline_access user_data vehicle_device_data vehicle_cmds vehicle_charging_cmds";

                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUrl),
                    new KeyValuePair<string, string>("scope", scope)
                };

                using (var client = new HttpClient())
                {
                    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                    tokenRequest.Content = new FormUrlEncodedContent(formData);
                    var tokenResponse = await client.SendAsync(tokenRequest);
                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                        var responseAsObject = JsonSerializer.Deserialize<TokenResponse>(tokenJson);
                        string UserToken = responseAsObject.access_token;
                        await StoreToken(responseAsObject, jsRuntime);
                        return UserToken;
                    }
                    else
                    {
                        return null;
                    }
                }
        }
        public static async Task<string> StoreToken(TokenResponse response, IJSRuntime jsRuntime)
        {
            var localStorage = new LocalStorageService(jsRuntime);
            await localStorage.SetItemAsync("UserToken", response.access_token);
            await localStorage.SetItemAsync("UserTokenExpiresAt", DateTime.Now.AddSeconds(response.expires_in));
            return string.Empty;
        }
    }
}
