using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Linq;
using System.Diagnostics;

public class GetTag
{
    private readonly HttpClient _httpClient;

    public GetTag(string accessToken)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<string> GetVehicleId()
    {
        try
        {
            // Make an API request to get the list of vehicles
            var response = await _httpClient.GetAsync("https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles");
            Debug.Print("Response Content: " + await response.Content.ReadAsStringAsync());
            Debug.Print($"Response: {response.Content}");
            Debug.Print($"httpClient: {_httpClient.DefaultRequestHeaders}");
            // ...
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Debug.Print($"Response Content: {content}");

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
                                if (firstVehicle.TryGetProperty("vehicle_id", out var vehicleId))
                                {
                                    if (vehicleId.ValueKind == JsonValueKind.Number)
                                    {
                                        long vehicleIdValue = vehicleId.GetInt64();
                                        Debug.Print($"Vehicle ID (vehicle_id): {vehicleIdValue}");
                                        return vehicleIdValue.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // If the code reaches here, there was no valid vehicle ID found in the response
            Debug.Print("No valid vehicle ID found in the response.");

        }
        catch (Exception ex)
        {
            Debug.Print($"Exception: {ex.Message}");
            return null;
        }

        return null;
    }
    public async Task<string> HonkHornAsync(string vehicleTag)
    {
        try
        {
            var wakeUpResponse = await _httpClient.PostAsync($"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/5YJSA7E18HF180633/wake_up", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

            if (!wakeUpResponse.IsSuccessStatusCode)
            {
                Debug.Print($"Error waking up vehicle: {wakeUpResponse.StatusCode}");
                return null;
            }
            // Wait for vehicle to wake up
            bool isVehicleAwake = false;
            int attempts = 0;
            const int maxAttempts = 6;
            //Attempts to check if vehicle is awake
            while (!isVehicleAwake && attempts < maxAttempts)
                {
                    attempts++;
                    await Task.Delay(5000); // Wait for 5 seconds before checking status

                    var vehicleStateResponse = await _httpClient.GetAsync($"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/5YJSA7E18HF180633/vehicle_data");
                    if (vehicleStateResponse.IsSuccessStatusCode)
                    {
                        var vehicleStateContent = await vehicleStateResponse.Content.ReadAsStringAsync();
                        Debug.Print($"Vehicle Content: {vehicleStateContent}");
                        isVehicleAwake = true;
                    }
                    else
                    {
                        Debug.Print($"Error fetching vehicle state: {vehicleStateResponse.StatusCode}");
                    }
                }

            if (!isVehicleAwake)
            {
                Debug.Print("Vehicle did not wake up in time.");
                return null;
            }

            //Vehicle is awake, attempt to honk horn
            string endpoint = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/5YJSA7E18HF180633/command/honk_horn";
            //string endpoint = $"https://fleet-api.prd.na.vn.cloud.tesla.com/api/1/vehicles/5YJSA7E18HF180633/command/actuate_trunk";
            var honkResponse = await _httpClient.PostAsync(endpoint, new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
            if (honkResponse.IsSuccessStatusCode)
            {
                var content = await honkResponse.Content.ReadAsStringAsync();
                Debug.Print("Horn honked successfully.");
                return content;
            }
            else
            {
                Debug.Print($"Error honking horn: {honkResponse.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.Print($"Exception: {ex.Message}");
            return null;
        }
    }
}
public class VehicleResponse
{
    public List<VehicleInfo> Response { get; set; }
}

public class VehicleInfo
{
    public long IdS { get; set; }
}
