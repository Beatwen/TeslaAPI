using System.Net.Http.Headers;
using System.Net.Http;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.WebRequestMethods;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using TeslaAPI.Component.Commands; 

namespace TeslaAPI.Component
{
    static class CommandsMethod
    {
        public static async Task Honk(string VIN, string token, VehicleDataResponse vehicleData, Command command)
        {
            string URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/honk_horn";
            await NewhttpClient(VIN, URL, token, vehicleData);
        }
        public static async Task Flash(string VIN, string token, VehicleDataResponse vehicleData, Command command)
        {
            string URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/flash_lights";
            await NewhttpClient(VIN, URL, token, vehicleData);
        }
        public static async Task SetSentryMode(string VIN, string token, VehicleDataResponse vehicleData, Command command)
        {
            bool onOff = false;
            if (vehicleData.response.vehicle_state.sentry_mode_available && vehicleData.response.vehicle_state.sentry_mode == false)
            {
                onOff = true;
            }
            string URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/set_sentry_mode";
            string data = JsonSerializer.Serialize(new { on = onOff });
            var response = await NewhttpClient(VIN, URL, token, vehicleData, data);
        }
        public static async Task Unlock(string VIN, string token, VehicleDataResponse vehicleData, Command command)
        {
            string URL;
            if (vehicleData.response.vehicle_state.locked)
            {
                Debug.Print("Unlocking the car");
                URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/door_unlock";
                
            }
            else
            {
                Debug.Print("Locking the car");
                URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/door_lock";
                
            }
            var response = await NewhttpClient(VIN, URL, token, vehicleData);
            if (response != null) { _=command.DisplayName == "Unlock" ? command.DisplayName = "Lock" : command.DisplayName = "Unlock"; }
        }
        public static async Task OpenChargePort(string VIN, string token, VehicleDataResponse vehicleData, Command command)
        {
            string URL;
            if (vehicleData.response.charge_state.charge_port_door_open)
            {
                URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/charge_port_door_open";
            }
            else
            {
                URL = $"https://fleet-api.prd.eu.vn.cloud.tesla.com/api/1/vehicles/{VIN}/command/charge_port_door_close";
            }
            var response = await NewhttpClient(VIN, URL, token, vehicleData);
            if (response != null) { _=command.DisplayName == "OpenChargePort" ? command.DisplayName = "CloseChargePort" : command.DisplayName = "OpenChargePort"; }
        }
        public static async Task<string?> NewhttpClient(string VIN, string URL, string token, VehicleDataResponse vehicleData, string? data = null)
        {
            if (vehicleData == null) 
                return "";
            var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.PostAsync(URL, new StringContent($"{data}", System.Text.Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.Print("Command Sent !");
                    return content;
                }
                else
                {
                    Debug.Print("failed");
                    return null;
                }
            }
        }
    }