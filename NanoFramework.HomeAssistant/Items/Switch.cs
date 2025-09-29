using System;
using System.Text;

namespace NanoFramework.HomeAssistant.Items
{
    public class Switch : HomeAssistantItem
    {
        public Switch(string switchName, string initialState)
            : base(initialState)
        {
            this.switchName = switchName;
        }

        public override string GetDiscoveryTopic() => $"homeassistant/switch/{homeAssistant.DeviceName.Replace(" ", "-")}/{switchName.Replace(" ", "-")}/config";
        public override string GetCommandTopic() => $"nanoframework/switches/{switchName.Replace(" ", "-")}/set";
        public override string GetStateTopic() => $"nanoframework/switches/{switchName.Replace(" ", "-")}/state";
        public override string GetAvailabilityTopic() => $"nanoframework/{switchName.Replace(" ", "-")}/state";

        public override string ToDiscoveryMessage()
        {
            return "{"
                + "\"name\": \"" + switchName + " switch\","
                + "\"unique_id\": \"" + switchName.Replace(" ", "-") + "-switch\","
                + "\"state_topic\": \"" + GetStateTopic() + "\","
                + "\"command_topic\": \"" + GetCommandTopic() + "\","
                + "\"availability_topic\": \"" + GetAvailabilityTopic() + "\","
                + "\"device\": { \"identifiers\": [ \"" + homeAssistant.DeviceName + "\" ] }"
                + "}";
        }

        private readonly string switchName;
    }
}