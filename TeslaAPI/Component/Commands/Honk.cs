using System.Diagnostics;
using System.Net.Http;

namespace TeslaAPI.Component.Commands
{
    public class Honk
    {
        //async Task Horn()
        //{
        //    string accessTokenJSON = token;
        //    accessToken = GetAccessTokenValue(accessTokenJSON);
        //    Debug.Print("access Token = " + accessToken);
        //    var horn = new GetTag(accessToken);
        //    Debug.Print("vehicleTag" + vehicleTag);
        //    string hornReturn = await horn.HonkHornAsync(vehicleTag);
        //    Debug.Print("L'id voiture : " + hornReturn);
        //}
        public async Task<string> HonkHornAsync(string vehicleTag)
        {
            try
            {
                var _httpClient = new HttpClient();
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
}
