using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Agents;
using MTConnect.Streams;
using MTConnect.Clients.Rest;
using MTConnect.Observations.Events.Values;
using MTConnect.Http;
using MTConnect.Models;
using MTConnect.Observations.Samples.Values;
using MTConnect.Models.DataItems;

var agent = new MTConnectAgent();
var server = new ShdrMTConnectHttpServer(agent);
server.Start();

var deviceModel = new DeviceModel("OKUMA-Lathe");
deviceModel.ObservationUpdated += (o, observation) =>
{
    if (observation != null)
    {
        Console.WriteLine($"{observation.DataItemId} => {observation.GetValue(ValueKeys.CDATA)}");

        agent.AddObservation(deviceModel.Name, new ObservationInput(observation));
    }
};

deviceModel.Availability = Availability.AVAILABLE;
deviceModel.Controller.EmergencyStop = EmergencyStop.TRIGGERED;

var xAxis = deviceModel.Axes.GetXAxis();
xAxis.MachinePosition = new PositionModel 
{ 
    Actual = new PositionValue(12.3456)
};
xAxis.Motor.Temperature = new TemperatureValue(35.12);

agent.AddDevice(deviceModel);

var observationInputs = new List<IObservationInput>();
foreach (var observation in deviceModel.GetObservations())
{
    agent.AddObservation(deviceModel.Name, new ObservationInput(observation));
}


var rnd = new Random();

while (true)
{
    Console.ReadLine();

    deviceModel.Availability = Availability.AVAILABLE;
    deviceModel.Controller.EmergencyStop = EmergencyStop.ARMED;

    xAxis.MachinePosition = new PositionModel
    {
        Actual = new PositionValue(98.7654)
    };
    xAxis.Motor.Temperature = new TemperatureValue(rnd.NextDouble());

    var path1 = "path1";
    deviceModel.Controller.GetPath(path1).Execution = Execution.ACTIVE;

    var mainProgram = deviceModel.Controller.GetPath(path1).MainProgram;
    mainProgram.Program = "Testing.NC";
    deviceModel.Controller.GetPath(path1).MainProgram = mainProgram;

    agent.AddDevice(deviceModel);
}

