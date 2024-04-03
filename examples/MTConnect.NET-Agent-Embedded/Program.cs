using MTConnect.Agents;
using MTConnect.Applications;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Events;


// Run Agent Application
var app = new MTConnectAgentApplication();
app.Run(args, true);


// [ Module Configuration ]
// This is where you can create a custom configuration section
public class ModuleConfiguration
{
    public string DeviceUuid { get; set; }
    public string DeviceName { get; set; }
}



// [ Agent Module ]
// This is the module that runs your code
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


    /// <summary>
    /// This method is run when the module starts and is used to define the device.
    /// You can build your device model through code or read from a static file
    /// </summary>
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


    /// <summary>
    /// [ Read from PLC ]
    /// This is where you can read from a PLC or other data source. This method is called at the interval set in the module configuration "readInterval"
    /// </summary>
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
}