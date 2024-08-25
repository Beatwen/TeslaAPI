namespace TeslaAPI.Component
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Microsoft.JSInterop;

    public class PartnerToken
    {
        public static async Task<string?> GetPartnerToken(IJSRuntime jsRuntime)
        {
            var filePath = "Data/PartnerToken.json";
            if (File.Exists(filePath))
            {
                var jsonContent = File.ReadAllText(filePath);
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonContent);
                bool valid = IsTokenStillValid(tokenResponse);
                if (tokenResponse != null && valid)
                {
                    return tokenResponse.access_token;
                }
                else
                {
                    return await GeneratePartnerToken();
                }
            }
            else
            {
                return await GeneratePartnerToken();
            }
        }
        static bool IsTokenStillValid(TokenResponse? tokenResponse)
        {
            Debug.Print("Checking if token is still valid");
            var expiresAt = DateTime.Now.AddSeconds(tokenResponse.expires_in);
            return expiresAt > DateTime.Now;
        }
        static async Task<string?> GeneratePartnerToken()
        {
            var clientCredentials = GetClientID();
            string clientId = clientCredentials[0];
            string clientSecret = clientCredentials[1];
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
                    if (responseContent != null)
                    {
                        TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                        StoreToken(tokenResponse);
                        return tokenResponse.access_token;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        private static void StoreToken(TokenResponse tokenResponse)
        {
            File.WriteAllText("Data/PartnerToken.json", JsonSerializer.Serialize(tokenResponse));
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
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }
    public class ErrorResponse
    {
        public string error_code { get; set; }
        public string error_description { get; set; }
    }

}
