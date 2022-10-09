using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System.Text;
using MTConnect.Clients.Mqtt;


var topics = new List<string>();

//var topic = "MTConnect/Devices/#/L2p1/#";
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/#");
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/Linear/L2x1/#");
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/Linear/L2z1/#");
//var topic = "MTConnect/Devices/Device/OKUMA.Lathe.123456/Observations/Path/L2p1/#";
//var topic = "MTConnect/Devices/Device/OKUMA.Lathe.123456/Observations/Device/#";


var client = new MTConnectMqttClient("localhost", topics: topics);
client.DeviceReceived += (s, o) =>
{
    Console.WriteLine($"Device Received : {o.Uuid}");
};
client.ObservationReceived += (s, o) =>
{
    Console.WriteLine($"Observation Received : {o.DataItemId}");
};
client.AssetReceived += (s, o) =>
{
    Console.WriteLine("Asset Received");
};

client.Start();

Console.WriteLine("Press enter to exit.");
Console.ReadLine();

client.Stop();