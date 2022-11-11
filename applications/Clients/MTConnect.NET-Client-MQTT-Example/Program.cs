using MTConnect.Clients.Mqtt;
using MTConnect.Configurations;
using MTConnect.Observations;


var topics = new List<string>();

//var topic = "MTConnect/Devices/#/L2p1/#";
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/#");
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/Linear/L2x1/#");
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/Linear/L2z1/#");
//var topic = "MTConnect/Devices/Device/OKUMA.Lathe.123456/Observations/Path/L2p1/#";
//var topic = "MTConnect/Devices/Device/OKUMA.Lathe.123456/Observations/Device/#";

var configuration = new MTConnectMqttClientConfiguration();
configuration.Server = "localhost";
configuration.Port = 1883;

var client = new MTConnectMqttClient(configuration, topics: topics);
client.DeviceReceived += (s, o) =>
{
    Console.WriteLine($"Device Received : {o.Uuid}");
};
client.ObservationReceived += (s, o) =>
{
    Console.WriteLine($"Observation Received : {o.DataItemId} :: {o.GetValue(ValueKeys.Result)}");
};
client.AssetReceived += (s, o) =>
{
    Console.WriteLine("Asset Received");
};

await client.StartAsync();

Console.WriteLine("Press enter to exit.");
Console.ReadLine();

await client.StopAsync();