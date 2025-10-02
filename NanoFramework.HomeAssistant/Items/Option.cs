using System;
using System.Text;

namespace NanoFramework.HomeAssistant.Items
{
    public class Option : HomeAssistantItem
    {
        private readonly string optionName;
        private readonly string[] options;

        internal Option(HomeAssistant homeAssistant, string optionName, string[] options, string initialState) : base(homeAssistant, initialState)
        {
            this.homeAssistant = homeAssistant;
            this.optionName = optionName;
            this.options = options;
        }

        public override string GetDiscoveryTopic() => $"homeassistant/select/{homeAssistant.DeviceName.Replace(" ", "-")}/{optionName.Replace(" ", "-")}/config";
        public override string GetCommandTopic() => $"nanoframework/switches/{homeAssistant.DeviceName.Replace(" ", "-")}/{optionName.Replace(" ", "-")}/set";
        public override string GetStateTopic() => $"nanoframework/switches/{homeAssistant.DeviceName.Replace(" ", "-")}s/{optionName.Replace(" ", "-")}/state";
        public override string GetAvailabilityTopic() => $"nanoframework/{homeAssistant.DeviceName.Replace(" ", "-")}/{optionName.Replace(" ", "-")}/availability";

        public override string ToDiscoveryMessage()
        {
            return "{"
                + "\"name\": \"" + optionName + " switch\","
                + "\"unique_id\": \"" + homeAssistant.DeviceName.Replace(" ", "-") + "-" + optionName.Replace(" ", "-") + "-option\","
                + "\"state_topic\": \"" + GetStateTopic() + "\","
                + "\"command_topic\": \"" + GetCommandTopic() + "\","
                + "\"options\": [ \"" + StringExtentionMethods.Join("\", \"", options) + "\" ],"
                + "\"availability_topic\": \"" + GetAvailabilityTopic() + "\","
                + "\"device\": { \"identifiers\": [ \"" + homeAssistant.DeviceName + "\" ] }"
                + "}";
        }
    }
}