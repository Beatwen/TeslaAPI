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
        static readonly string BaseUrl = "https://engaging-camel-suitably.ngrok-free.app";
        static readonly string RedirectUrl = $"{BaseUrl}/oauth-callback";
        static readonly string tokenEndpoint = "https://auth.tesla.com/oauth2/v3/token";
        static readonly string Scope = "openid offline_access user_data vehicle_device_data vehicle_cmds vehicle_charging_cmds";
        public static async Task<string?> GetUserToken(NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            LocalStorageService localStorageService = new LocalStorageService(jsRuntime);
            string? token = await localStorageService.GetItemAsync<string>("UserToken");
            if (token != null && await IsTokenStillValid(localStorageService)) 
            {
                return token;
            }
            else if (token != null && await IsTokenStillValid(localStorageService) == false)
            {
                string? refreshToken = await localStorageService.GetItemAsync<string>("RefreshToken");
                token = await RefreshToken(refreshToken, jsRuntime, navigationManager);
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
            var clientCredentials = GetClientID();
            string clientId = clientCredentials[0];
            string codeVerifier = Code.GenerateRandomString(86);
            string state = Code.GenerateRandomString(32);
            string authorizeUrl = $"https://auth.tesla.com/oauth2/v3/authorize?&client_id={clientId}&locale=en-US&prompt=login&redirect_uri={RedirectUrl}&response_type=code&scope=openid%20offline_access%20user_data%20vehicle_device_data%20vehicle_cmds%20vehicle_charging_cmds&state={state}";
            using (var client = new HttpClient())
            {
                navigationManager.NavigateTo(authorizeUrl);
            }
        }
        public static void RedirectUserToIndex(NavigationManager navigationManager, IJSRuntime jSRuntime)
        {
            using (var client = new HttpClient())
            {
                navigationManager.NavigateTo(BaseUrl);
            }
        }
        public static async Task<string?> GenerateUserToken(string authorizationCode, IJSRuntime jsRuntime)
        {
            var clientCredentials = GetClientID();
            string ClientId = clientCredentials[0];
            string ClientSecret = clientCredentials[1];
                var formData = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "authorization_code"),
                    new("code", authorizationCode),
                    new("client_id", ClientId),
                    new("client_secret", ClientSecret),
                    new("redirect_uri", RedirectUrl),
                    new("scope", Scope)
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
                        if (responseAsObject == null) { return null;}
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
        public static async Task<string?> RefreshToken(string refreshToken, IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            var clientCredentials = GetClientID();
            string ClientId = clientCredentials[0];
            string ClientSecret = clientCredentials[1];
            var formData = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "refresh_token"),
                    new("refresh_token", refreshToken),
                    new("client_id", ClientId),
                    new("client_secret", ClientSecret),
                    new("scope", Scope)
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
                    if (responseAsObject == null) { return null; }
                    string userToken = responseAsObject.access_token;
                    await StoreToken(responseAsObject, jsRuntime);
                    return userToken;
                }
                else
                {
                    RedirectUser(navigationManager, jsRuntime);
                    return null;
                }
            }
        }
        public static async Task<string> StoreToken(TokenResponse response, IJSRuntime jsRuntime)
        {
            var localStorage = new LocalStorageService(jsRuntime);
            await localStorage.SetItemAsync("UserToken", response.access_token);
            await localStorage.SetItemAsync("RefreshToken", response.refresh_token);
            await localStorage.SetItemAsync("UserTokenExpiresAt", DateTime.Now.AddSeconds(response.expires_in));
            return string.Empty;
        }
        public static List<string> GetClientID()
        {
            string json = System.IO.File.ReadAllText("Data/ClientID.json");
            var config = JsonSerializer.Deserialize<ClientConfig>(json);
            List<string> clientIDs = new()
            {
                config.ClientId,
                config.ClientSecret
            };
            return clientIDs;
        }
    }
    public class ClientConfig
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
    }
}
