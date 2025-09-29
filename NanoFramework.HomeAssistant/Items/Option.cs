using System;
using System.Text;

namespace NanoFramework.HomeAssistant.Items
{
    public class Option : HomeAssistantItem
    {
        private readonly string optionName;
        private readonly string[] options;

        public Option(string optionName, string[] options, string initialState) : base(initialState)
        {
            this.optionName = optionName;
            this.options = options;
        }

        public override string GetDiscoveryTopic() => $"homeassistant/select/{homeAssistant.DeviceName.Replace(" ", "-")}/{optionName.Replace(" ", "-")}/config";
        public override string GetCommandTopic() => $"nanoframework/switches/{optionName.Replace(" ", "-")}/set";
        public override string GetStateTopic() => $"nanoframework/switches/{optionName.Replace(" ", "-")}/state";
        public override string GetAvailabilityTopic() => $"nanoframework/{optionName.Replace(" ", "-")}/state";

        public override string ToDiscoveryMessage()
        {
            return "{"
                + "\"name\": \"" + optionName + " switch\","
                + "\"unique_id\": \"" + optionName.Replace(" ", "-") + "-option\","
                + "\"state_topic\": \"" + GetStateTopic() + "\","
                + "\"command_topic\": \"" + GetCommandTopic() + "\","
                + "\"options\": [ \"" + StringExtentionMethods.Join("\", \"", options) + "\" ],"
                + "\"availability_topic\": \"" + GetAvailabilityTopic() + "\","
                + "\"device\": { \"identifiers\": [ \"" + homeAssistant.DeviceName + "\" ] }"
                + "}";
        }
    }
}