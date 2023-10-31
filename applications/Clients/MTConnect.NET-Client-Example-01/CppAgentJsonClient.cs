using MTConnect.Clients;

var agentUrl = "localhost:5001";

var client = new MTConnectHttpClient(agentUrl);
client.Interval = 100;
client.Heartbeat = 10000;
client.ContentType = "application/json";
client.DocumentFormat = "json-cppagent";
client.DeviceReceived += (sender, device) =>
{
    Console.WriteLine($"Device Received : {device.Uuid}");
};
client.ObservationReceived += (sender, observation) =>
{
    Console.WriteLine($"Observation Received : {observation.DataItemId} = {observation.GetValue("Result")} @ {observation.Timestamp}");
};
client.AssetReceived += (sender, asset) =>
{
    Console.WriteLine($"Asset Received : {asset.AssetId}");
};

client.Start();

Console.ReadLine();
