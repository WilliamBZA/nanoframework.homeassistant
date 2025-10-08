# nanoFramework.HomeAssistant.MqttDiscovery

Easily integrate your **.NET nanoFramework** devices with **Home Assistant** using **MQTT Auto Discovery**.  
This library lets your devices automatically register entities like sensors and switches without any manual configuration in Home Assistant.

---

## Features

- Home Assistant **MQTT Auto Discovery** support  
- Simple API for defining **sensors**, **switches**, and other entities  
- Built-in support for **state updates** and **command handling**  
- Supports MQTT authentication  
- Compatible with **.NET nanoFramework** for ESP32 and similar devices  

---

## Installation

You can install the package via NuGet:

`nuget install nanoFramework.HomeAssistant.MqttDiscovery`

## Example usage

```csharp
using nanoFramework.HomeAssistant.MqttDiscovery;
using System.Device.Gpio;

// Create the Home Assistant client
var homeassistant = new HomeAssistant(
    "Second Test",
    "192.168.88.172",
    username: "homeassistant",
    password: "password");

// Example: Add a switch entity
var relay = homeassistant.AddSwitch("Switch number 1", "OFF");
relay.OnChange += (s, e) =>
{
    if (e == "ON")
    {
        relayPin.Write(PinValue.High);
    }
    else
    {
        relayPin.Write(PinValue.Low);
    }
};

// Example: Add a temperature sensor
var temperatureSensor = homeassistant.AddSensor("Temperature", "°C", "18", DeviceClass.Temperature);
temperatureSensor.OnChange += (s, e) =>
{
    Console.WriteLine($"Temperature changed to {e}");
};

// Connect to home assistant
homeassistant.Connect();

// Set the relay to off
relay.SetState("OFF"); // This publishes an MQTT event to the switch's state topic. Home assistant will show the new state of off.
relayPin.Write(PinValue.Low);

Thread.Sleep(Timeout.Infinite);
```

This example:

- Creates a Home Assistant connection over MQTT
- Adds a switch that responds to Home Assistant commands
- Adds a temperature sensor that reports values automatically

## Configuration

Make sure your Home Assistant installation has the MQTT integration enabled and is configured to allow Auto Discovery.

You can verify this by checking Configuration → Devices & Services → MQTT.

For more details: [Home Assistant MQTT Discovery Docs](https://www.home-assistant.io/docs/mqtt/discovery/)

## Entity Types Supported

- Sensors
  - All types defined on https://www.home-assistant.io/integrations/sensor/#device-class
- Switches
- Text inputs

## License

This project is licensed under the MIT License.

## Contributing

Contributions, issues, and feature requests are welcome!

Feel free to open an issue or pull request on the GitHub repository.