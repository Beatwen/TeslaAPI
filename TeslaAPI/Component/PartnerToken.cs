namespace TeslaAPI.Component
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Diagnostics;
    public class PartnerToken
    {
        public static async Task<string> GetPartnerToken()
        {
            Debug.Print("Generating partner access token...");
            string clientId = "285b750b0c21-49a2-a9af-c44c1f100566";
            string clientSecret = "ta-secret.rTLXH7fIyZwU5PTf";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Set the token request parameters
                    var tokenRequest = new
                    {
                        grant_type = "client_credentials",
                        client_id = clientId,
                        client_secret = clientSecret,
                        scope = ""
                    };

                    // Serialize the token request parameters to JSON
                    var tokenRequestJson = JsonSerializer.Serialize(tokenRequest);

                    // Create an HttpRequestMessage for the token request
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.tesla.com/oauth2/v3/token")
                    {
                        Content = new StringContent(tokenRequestJson, Encoding.UTF8, "application/json")
                    };

                    // Send the token request
                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response to obtain the partner access token
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                        string partnerAccessToken = tokenResponse.access_token;
                        Debug.Print("Partner Access Token: " + partnerAccessToken);
                        return partnerAccessToken;
                        
                    }
                    else
                    {
                        Debug.Print("Failed to obtain partner access token. Status code: " + response.StatusCode);

                        // Read the error response content
                        var errorResponseContent = await response.Content.ReadAsStringAsync();

                        try
                        {
                            // Deserialize the error response to an object (if it's in JSON format)
                            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorResponseContent);

                            // Access and print the error details
                            Debug.Print("Error Code: " + errorResponse.error_code);
                            Debug.Print("Error Description: " + errorResponse.error_description);
                        }
                        catch (JsonException)
                        {
                            // If the error response is not in JSON format, just print the raw content
                            Debug.Print("Error Response Content: " + errorResponseContent);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during token generation: " + ex.Message);
            }
            return null;
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
