using System;
using System.Text;

namespace NanoFramework.HomeAssistant.Items
{
    public abstract class HomeAssistantItem(HomeAssistant homeAssistant, string state)
    {
        public delegate string MessageAction(string message);

        public abstract string GetDiscoveryTopic();
        public abstract string ToDiscoveryMessage();

        public abstract string GetCommandTopic();
        public abstract string GetStateTopic();
        public abstract string GetAvailabilityTopic();

        public virtual string GetState()
        {
            return state;
        }

        public void OnSet(MessageAction action)
        {
            setAction = action;
        }

        public void Trigger(string message)
        {
            state = setAction?.Invoke(message);
            homeAssistant.StateChanged(this, state);
        }

        protected MessageAction setAction;
        protected HomeAssistant homeAssistant = homeAssistant;
    }
}