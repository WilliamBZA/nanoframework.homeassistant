using nanoFramework.Json;
using nanoFramework.Networking;
using NanoFramework.HomeAssistant;
using NanoFramework.HomeAssistant.Items;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using TimeTrigger;

namespace TestSwitch
{
    public class Program
    {
        public static void Main()
        {
            var controller = new GpioController();
            var relayPin = controller.OpenPin(13, PinMode.Output);
            relayPin.Write(PinValue.Low);

            var state = new CurrentState();
            using (var stateFile = File.Open("I:\\state.json", FileMode.OpenOrCreate))
            {
                if (stateFile.Length > 0)
                {
                    var fileContent = new byte[stateFile.Length];
                    stateFile.Read(fileContent, 0, (int)stateFile.Length);

                    var stateString = UTF8Encoding.UTF8.GetString(fileContent, 0, fileContent.Length);
                    state = (CurrentState)JsonConvert.DeserializeObject(stateString, typeof(CurrentState));
                }
            }

            ConnectToWiFi();

            var homeassistent = new HomeAssistant("Pool Pump", "192.168.88.172", username: "homeassistant", password: "mindfree");

            var now = DateTime.UtcNow.AddHours(2);
            var today = now.Date;
            var onTime = today.Add(StringExtentionMethods.ParseTimeString(state.OnTime));
            var offTime = today.Add(StringExtentionMethods.ParseTimeString(state.OffTime));

            var isOnAtStartup = now >= onTime && now < offTime;
            if (isOnAtStartup)
            {
                relayPin.Write(PinValue.High);
            }

            var relay = homeassistent.AddSwitch("Pool Pump", isOnAtStartup ? "ON" : "OFF");
            relay.OnSet(message =>
            {
                if (message == "ON")
                {
                    relayPin.Write(PinValue.High);
                }
                else
                {
                    relayPin.Write(PinValue.Low);
                }

                return message;
            });

            var times = new[] {
                "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00",
                "14:30", "15:00", "15:30", "16:00", "16:30", "17:00", "17:30", "18:00", "18:30", "19:00", "19:30", "20:00", "20:30",
                "21:00", "21:30", "22:00", "22:30", "23:00", "23:30", "00:00", "00:30", "01:00", "01:30", "02:00", "02:30", "03:00",
                "03:30", "04:00", "04:30", "05:00", "05:30", "06:00", "06:30", "07:00", "07:30"
            };

            var timeManager = new TimeSwitch(state.OnTime, state.OffTime, isOn =>
            {
                relay.Trigger(isOn ? "ON" : "OFF");
                Console.WriteLine($"{DateTime.UtcNow} State changed to {(isOn ? "On" : "Off")} ");
            });
            timeManager.Start();

            var timeOnOptions = homeassistent.AddOption("On time", times, state.OnTime);
            timeOnOptions.OnSet(message =>
            {
                Console.WriteLine($"On time set to {message}");
                state.OnTime = message;
                timeManager.UpdateOnTime(message);

                SaveCurrentState(state);

                return message;
            });

            var timeOffOptions = homeassistent.AddOption("Off time", times, state.OffTime);
            timeOffOptions.OnSet(message =>
            {
                Console.WriteLine($"Off time set to {message}");
                state.OffTime = message;
                timeManager.UpdateOffTime(message);

                SaveCurrentState(state);

                return message;
            });

            homeassistent.Connect();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void SaveCurrentState(CurrentState state)
        {
            using (var stateFile = File.Open("I:\\state.json", FileMode.Create))
            {
                var stateString = JsonConvert.SerializeObject(state);
                var stateBytes = UTF8Encoding.UTF8.GetBytes(stateString);

                stateFile.Write(stateBytes, 0, stateBytes.Length);
            }
        }

        private static void ConnectToWiFi()
        {
            var connected = WifiNetworkHelper.Reconnect(requiresDateTime: true);
            if (connected)
            {
                var ipAddress = IPGlobalProperties.GetIPAddress().ToString();
                Console.WriteLine($"Connected {ipAddress}");
            }
        }
    }
}
