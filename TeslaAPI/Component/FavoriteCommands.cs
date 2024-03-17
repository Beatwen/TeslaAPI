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
                "Unlock",
                "OpenChargePort",
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
        private string? _displayName;
        public required string Name { get; set; }
        public string DisplayName
        {
            get => _displayName ?? Name;
            set => _displayName = value;
        }
        public bool Selected { get; set; }
        public int Value { get; set; }
        public int BoolValue { get; set; }
    }
}
