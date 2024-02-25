using MudBlazor;

namespace TeslaAPI.Component
{
    public class FavoriteCommands
    {
        public List<Command> Commands { get; set; } = new();
        public List<string> BaseTasks { get; set; } = new List<string>
            {
                "Honk",
                "Flash",
                "Lock",
                "Unlock",
                "OpenChargePort",
                "CloseChargePort",
                "StartHVAC",
                "StopHVAC",
                "SetChargeLimit",
                "SetTemperature",
                "SetSentryMode",
                "WakeUp",
                "OpenTrunk",
                "OpenFrunk",
                "StartCharging",
                "StopCharging",
                "SetValetMode",
                "ResetValetPin",
                "SetSunRoof",
                "StartSoftwareUpdate",
                "CancelSoftwareUpdate"
            };
    }

    public class Command
    {
        public required string Name { get; set; }
        public bool Selected { get; set; }
        public int Value { get; set; }
        public int BoolValue { get; set; }
    }
}
