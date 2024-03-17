using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TeslaAPI.Component
{
    public static class RegisterVehicleClass
    {
        public static async Task RegisterVehicle(string accessToken)
     {
         try
         {
             Debug.Print("Access Token = " + accessToken);

                     if (string.IsNullOrWhiteSpace(accessToken))
                     {
                         Debug.Print("Access token is missing. Please obtain the access token first.");
                         return;
                     }

                    string registerEndpoint = "https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/partner_accounts";
                    var payload = new
                    {
                        domain = "cod-precious-elephant.ngrok-free.app"
                    };
                    var payloadJson = JsonSerializer.Serialize(payload);
                     using (var httpClient = new HttpClient())
                     {
                         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        var response = await httpClient.PostAsync(registerEndpoint, new StringContent(payloadJson, Encoding.UTF8, "application/json"));

                                             if (response.IsSuccessStatusCode)
                                             {
                                                 Debug.Print("Vehicle registration successful.");
                                             }
                                             else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        Debug.Print("Response Content: " + await response.Content.ReadAsStringAsync());
                        var errorResponseContent = await response.Content.ReadAsStringAsync();

                        try
                        {
                            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorResponseContent);
                            Debug.Print("Error Code: " + errorResponse.error_code);
                            Debug.Print("Error Description: " + errorResponse.error_description);
                        }
                        catch (JsonException)
                        {
                            Debug.Print("Error Response Content: " + errorResponseContent);
                        }
                    }
                    else
                    {
                        Debug.Print("Vehicle registration failed. Status code: " + (int)response.StatusCode);
                        Debug.Print(response.ReasonPhrase + response.Content.ReadAsStringAsync());
                    }
                     }
                 }
                 catch (Exception ex)
                 {
                     Debug.Print("Exception during vehicle registration: " + ex.Message);
                 }
             }
    }
}
