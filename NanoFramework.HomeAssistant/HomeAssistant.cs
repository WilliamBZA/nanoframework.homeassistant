using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using NanoFramework.HomeAssistant.Items;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NanoFramework.HomeAssistant
{
    public class HomeAssistant : IDisposable
    {
        public HomeAssistant(string deviceName, string brokerIp, int port = 1883, string username = "", string password = "")
        {
            DeviceName = deviceName;
            this.brokerIp = brokerIp;
            this.port = port;
            this.username = username;
            this.password = password;

            items = new ArrayList();
        }

        public string DeviceName { get; private set; }

        public void AddItem(HomeAssistantItem item)
        {
            items.Add(item);
            item.SetParent(this);
        }

        public void Connect()
        {
            client = new MqttClient(brokerIp, port, false, null, null, MqttSslProtocols.None);
            client.ProtocolVersion = MqttProtocolVersion.Version_3_1;

            client.MqttMsgPublishReceived += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
                Console.WriteLine($"Received message on topic {e.Topic}: {message}");

                foreach (HomeAssistantItem item in items)
                {
                    if (item.GetCommandTopic() == e.Topic)
                    {
                        item.Trigger(message);
                    }
                }
            };

            client.Connect(DeviceName, username, password);

            Console.WriteLine("Connected to MQTT Broker");

            PublishAutoDiscovery();
        }

        public void PublishAutoDiscovery()
        {
            foreach (HomeAssistantItem item in items)
            {
                client.Subscribe(new[] { item.GetCommandTopic() }, new[] { MqttQoSLevel.AtLeastOnce });

                var topic = item.GetDiscoveryTopic();
                var message = item.ToDiscoveryMessage();

                Console.WriteLine($"Publishing to '{topic}': {message}");

                client.Publish(topic, Encoding.UTF8.GetBytes(message), null, null, MqttQoSLevel.AtMostOnce, true);
                Console.WriteLine($"Published Auto Discovery for {topic}");
            }

            Thread.Sleep(10);
            foreach (HomeAssistantItem item in items)
            {
                var topic = item.GetAvailabilityTopic();
                client.Publish(topic, Encoding.UTF8.GetBytes("online"), null, null, MqttQoSLevel.AtMostOnce, true);
                client.Publish(item.GetStateTopic(), Encoding.UTF8.GetBytes(item.GetState()), null, null, MqttQoSLevel.AtLeastOnce, true);
            }
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        internal void StateChanged(HomeAssistantItem item, string state)
        {
            client.Publish(item.GetStateTopic(), Encoding.UTF8.GetBytes(state), null, null, MqttQoSLevel.AtMostOnce, true);
        }

        MqttClient client;

        private string brokerIp;
        private int port;
        private string username;
        private string password;

        private ArrayList items;
    }
}