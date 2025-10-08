using System;
using System.Text;

namespace nanoFramework.HomeAssistant.MqttDiscovery.Items
{
    public abstract class HomeAssistantItem(HomeAssistant homeAssistant, string state)
    {
        public delegate void MessageAction(object sender, string message);
        public event MessageAction OnChange;

        public abstract string GetDiscoveryTopic();
        public abstract string ToDiscoveryMessage();

        public abstract string GetCommandTopic();
        public abstract string GetStateTopic();
        public abstract string GetAvailabilityTopic();

        public virtual string GetState()
        {
            return state;
        }

        public void Trigger(string message)
        {
            if (message != state && OnChange != null)
            {
                OnChange(this, message);
            }

            state = message;
            homeAssistant.StateChanged(this, state);
        }

        protected MessageAction setAction;
        protected HomeAssistant homeAssistant = homeAssistant;
    }
}