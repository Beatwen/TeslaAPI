using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace TeslaAPI.Component.Commands
{
    public static class WakeUp
    {
        public static async Task<VehicleDataResponse> WakeItUp(string vehicleVIN, string userToken)
        {
            try
            {
                var _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var wakeUpResponse = await _httpClient.PostAsync($"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/5YJSA7E18HF180633/wake_up", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
                if (!wakeUpResponse.IsSuccessStatusCode)
                {
                    Debug.Print($"Error waking up vehicle: {wakeUpResponse.StatusCode}");
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
                        var vehicleResponse = JsonSerializer.Deserialize<VehicleDataResponse>(vehicleStateContent);
                        Debug.Print($"Vehicle Content: {vehicleStateContent}");
                        //int? batteryLevel = vehicleResponse.Response.;
                        long? userId = vehicleResponse?.response?.user_id;
                        Debug.Print($"User ID: {userId}");
                        int? batteryLevel = vehicleResponse.response.charge_state.battery_level;
                        isVehicleAwake = true;
                        return vehicleResponse;
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
            }
            catch (Exception ex)
            {
                Debug.Print($"Exception: {ex.Message}");
                return null;
            }
            return null;
        }
    }
    public class VehicleDataResponse
    {
        public ResponseData response { get; set; }
    }
    public class ResponseData
    {
        public long user_id { get; set; }
        public ChargeState charge_state { get; set; }
    }
    public class ChargeState
    {
        public int battery_level {  get; set; }
        public string preconditioning_times { get; set; }
        public bool preconditioning_enable { get; set; }


    }
}
