# Embedded MTConnect Agent Example
This is an example of an application that has an embedded MTConnect Agent. This means that the Agent is part of the same application as the data source. This can be used to build what would typically be an "Adapter" although this eliminates the need for using the SHDR protocol or any additional TCP connections.

## Create .NET Project
Create a .NET Console project:

![Visual-Studio-Create-Project](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/vs-create-console-project.png)

## Install Nuget Pacakge
Install the [MTConnect.NET-Applications-Agents](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents) Nuget package:
```
dotnet add package MTConnect.NET-Applications-Agents
```

## Edit Program.cs
For this example, all user code is in the Program.cs file. For more advanced/complicated applications, this code can be distributed among several files as a normal .NET project.

The user code will be written as an Agent Module which allows us to use the **MTConnectInputAgentModule** base class that provides access to the underlying Agent, Configuration File, etc.


### Create Instance of MTConnectAgentApplication
```c#
// Run Agent Application
var app = new MTConnectAgentApplication();
app.Run(args, true);
```

### Create Custom Configuration class
You can use this class to configure your user code using the same **agent.config.yaml** configuration file that a standalone agent uses. This configuration can be used to set the Device UUID and Name and also the address, port, etc. of the PLC or other data source.

```c#
public class ModuleConfiguration
{
    public string DeviceUuid { get; set; }
    public string DeviceName { get; set; }
}
```

### Add User Code
The user code is where you read from a data source (ex. PLC, Database, other MTConnect Agent, etc.). This can also be where you can transform data as needed.

```c#
public class Module : MTConnectInputAgentModule
{
    public const string ConfigurationTypeId = "demo"; // This must match the module section in the 'agent.config.yaml' file
    public const string DefaultId = "Demo Module"; // The ID is mainly just used for logging.
    private readonly ModuleConfiguration _configuration;


    public Module(IMTConnectAgentBroker agent, object configuration) : base(agent)
    {
        Id = DefaultId;

        _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
    }
}
```

#### Create Device
Using the **Device** class, you can build the Device model programatically instead of reading from a static file (ex. devices.xml). This allows for dynamically adjusting the Device model based on the information read from the PLC. Examples of this could be the number of Axes, number of Paths, available Alarms, serial numbers for components such as Motors, Encoders, etc.

Component and DataItem IDs can be automatically generated or they can be manually set.

Below is a simple example of how a Device Model can be built.

```c#
protected override IDevice OnAddDevice()
{
    var device = new Device();
    device.Uuid = "7E647B2D-C6A3-40BF-9CE9-FB09834850C9";
    device.Id = "dev-001";
    device.Description = new Description()
    {
        Manufacturer = "ACME",
        Model = "dm-500"
    };

    // Add an Availability DataItem to the Device
    device.AddDataItem<AvailabilityDataItem>();

    AddController(device);

    AddAxes(device);

    return device;
}

private void AddController(Device device)
{
    // Create a Controller Component
    var controller = new ControllerComponent();

    // Add an EmergencyStop DataItem to the controller component
    controller.AddDataItem<EmergencyStopDataItem>();

    // Create a Path Component
    var path = new PathComponent();

    // Add Path DataItems
    path.AddDataItem<ControllerModeDataItem>();
    path.AddDataItem<ExecutionDataItem>();
    path.AddDataItem<ProgramDataItem>();
    path.AddDataItem<DateCodeDataItem>();
    path.AddDataItem<SystemDataItem>();

    // Add the Path Component as a child of the Controller Component
    controller.AddComponent(path);

    // Add the Controller Component to the Device
    device.AddComponent(controller);
}

private void AddAxes(Device device)
{
    // Create a Axes Component
    var axes = new AxesComponent();

    AddLinearAxis(axes, "X");
    AddLinearAxis(axes, "Y");
    AddLinearAxis(axes, "Z");

    // Add the Component to the Device
    device.AddComponent(axes);
}

private void AddLinearAxis(AxesComponent axesComponent, string name)
{
    // Create a Linear Component
    var axis = new LinearComponent();
    axis.Name = name;

    axis.AddDataItem<PositionDataItem>(PositionDataItem.SubTypes.PROGRAMMED);
    axis.AddDataItem<PositionDataItem>(PositionDataItem.SubTypes.ACTUAL);
    axis.AddDataItem<LoadDataItem>();

    // Add the Component to the AxesComponent
    axesComponent.AddComponent(axis);
}

```

#### Read from Data Source
The **OnRead()** method is called at an interval set in the configuration file using the "readInterval" parameter and is intended to be the method to use for reading from the Data Source.

Observations can be added based on the DataItem Type/SubType or by the DataItemId.

- Supports Sample, Event, and Condition Categories.
- Supports Value, DataSet, Table, and TimeSeries Representations.

```c#
protected override void OnRead()
{
    Log(MTConnect.Logging.MTConnectLogLevel.Information, "Read PLC Data");

    AddValueObservation<AvailabilityDataItem>(Availability.AVAILABLE);
    AddValueObservation<ControllerComponent, EmergencyStopDataItem>(EmergencyStop.ARMED);
    AddValueObservation<PathComponent, ProgramDataItem>("BRACKET.NC");
    AddValueObservation<PathComponent, DateCodeDataItem>(DateTime.Now);
    AddConditionObservation<PathComponent, SystemDataItem>(MTConnect.Observations.ConditionLevel.WARNING, "404", "This is an Alarm");


    AddValueObservation<LinearComponent, PositionDataItem>(0.0000, "X", PositionDataItem.SubTypes.PROGRAMMED);
    AddValueObservation<LinearComponent, PositionDataItem>(0.0002, "X", PositionDataItem.SubTypes.ACTUAL);
    AddValueObservation<LinearComponent, LoadDataItem>(2, "X");

    AddValueObservation<LinearComponent, PositionDataItem>(150.0000, "Y", PositionDataItem.SubTypes.PROGRAMMED);
    AddValueObservation<LinearComponent, PositionDataItem>(150.0001, "Y", PositionDataItem.SubTypes.ACTUAL);
    AddValueObservation<LinearComponent, LoadDataItem>(1.5, "Y");

    AddValueObservation<LinearComponent, PositionDataItem>(200.0000, "Z", PositionDataItem.SubTypes.PROGRAMMED);
    AddValueObservation<LinearComponent, PositionDataItem>(200.0003, "Z", PositionDataItem.SubTypes.ACTUAL);
    AddValueObservation<LinearComponent, LoadDataItem>(6.3, "Z");
}
```
