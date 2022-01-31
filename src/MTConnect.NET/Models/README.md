# Models

## Overview
Models replaces the traditional method of creating a static XML file that stores what the machine is capable of reporting with a fully dynamic method contained in the application code (.NET). This allows developers to read what is available from the machine’s PLC during runtime and can update the model as needed.

The goal of using Models is to both encourage the proper use of the MTConnect standard as well as promote the lesser used features of the standard so that more machine data is accessible through MTConnect.

Models features include :

- The ability to work with an object model instead of keeping up with ID's and Types
- Restricts values to only the values that are part of the MTConnect standard so that developers can be assured that their applications comply with the standard
- Extensible to allow custom types

```c#
// Create a new DeviceModel
var device = new DeviceModel(_deviceName);
device.Manufacturer = "Mazak";
device.Model = "HCN-10800";

// Set Availiability
device.Availability = Availability.AVAILABLE;

// Set Controller Variables
device.Controller.EmergencyStop = EmergencyStop.ARMED;
device.Controller.ControllerMode = ControllerMode.AUTOMATIC;
device.Controller.FunctionalMode = FunctionalMode.PRODUCTION;
device.Controller.SystemCondition = Condition.Fault("404", message: "Example Alarm Code");

// Set Path Variables
var path = device.Controller.GetPath("path1");
path.Execution = Execution.ACTIVE;

// Update the MTConnect Agent
await mtconnectAgent.AddDeviceModel(device);
```
