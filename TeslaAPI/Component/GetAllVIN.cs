using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Linq;
using System.Diagnostics;

public class GetAllVIN
{
    private readonly HttpClient _httpClient;

    public GetAllVIN(string accessToken)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<string?> GetVIN()
    {
        try
        {
            // Make an API request to get the list of vehicles

            var response = await _httpClient.GetAsync("https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Parse the JSON response into a JsonDocument
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    // Root element should be an object with a "response" property
                    if (doc.RootElement.TryGetProperty("response", out var responseArray))
                    {
                        // Check if "response" is an array
                        if (responseArray.ValueKind == JsonValueKind.Array)
                        {
                            // Check if the array is not empty
                            if (responseArray.GetArrayLength() > 0)
                            {
                                // Get the first element of the array
                                var firstVehicle = responseArray[0];

                                // Check if "vehicle_id" exists in the first element
                                if (firstVehicle.TryGetProperty("vin", out var vehicleVin))
                                {
                                    if (vehicleVin.ValueKind == JsonValueKind.String)
                                    {
                                        return vehicleVin.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Debug.Print("No valid vehicle ID found in the response.");

        }
        catch (Exception ex)
        {
            Debug.Print($"Exception: {ex.Message}");
            return null;
        }

        return null;
    }

}
public class VehicleResponse
{
    public required List<VehicleInfo> Response { get; set; }
}

public class VehicleInfo
{
    public long IdS { get; set; }
}
