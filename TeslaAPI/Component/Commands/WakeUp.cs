using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace TeslaAPI.Component.Commands
{
    public static class WakeUp
    {
        public static async Task<VehicleDataResponse?> WakeItUp(string vehicleVIN, string userToken)
        {
            try
            {
                var _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var wakeUpResponse = await _httpClient.PostAsync($"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{vehicleVIN}/wake_up", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
                if (!wakeUpResponse.IsSuccessStatusCode)
                {
                    Debug.Print($"Error waking up vehicle: {wakeUpResponse.StatusCode}");
                }
                bool isVehicleAwake = false;
                int attempts = 0;
                const int maxAttempts = 30;
                while (!isVehicleAwake && attempts < maxAttempts)
                {
                    attempts++;
                    await Task.Delay(1000);
                    var vehicleInfo = await GetVehicleInfo(vehicleVIN, userToken);
                    if (vehicleInfo != null)
                    {
                        isVehicleAwake = true;
                        return vehicleInfo;
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
        public static async Task<VehicleDataResponse?> GetVehicleInfo(string vehicleVIN, string userToken)
        {
            var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var vehicleStateResponse = await _httpClient.GetAsync($"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{vehicleVIN}/vehicle_data");
            if (vehicleStateResponse.IsSuccessStatusCode)
            {
                var vehicleStateContent = await vehicleStateResponse.Content.ReadAsStringAsync();
                var vehicleData = JsonSerializer.Deserialize<VehicleDataResponse>(vehicleStateContent);
                long? userId = vehicleData?.response?.user_id;
                int? batteryLevel = vehicleData?.response?.charge_state.battery_level;
                return vehicleData;
            }
            else
            {
                Debug.Print($"Error fetching vehicle state: {vehicleStateResponse.StatusCode}");
                return null;
            }
        }
    }
    public class VehicleDataResponse
    {
        public required ResponseData response { get; set; }
    }
    public class ResponseData
    {
        public long user_id { get; set; }
        public required int vehicle_id { get; set; }
        public required string vin { get; set; }
        public required ChargeState charge_state { get; set; }
        public required VehicleState vehicle_state { get; set; }
    }
    public class ChargeState
    {
        public int battery_level {  get; set; }
        public required string preconditioning_times { get; set; }
        public bool preconditioning_enable { get; set; }
    }
    public class VehicleState
    {
        public bool sentry_mode { get; set; }
        public bool sentry_mode_available { get; set; }
        public bool preconditioning_enable { get; set; }
        public bool remote_start { get; set; }
        public bool locked {  get; set; }
    }
}
