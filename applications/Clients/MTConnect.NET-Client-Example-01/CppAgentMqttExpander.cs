using MTConnect.Clients;

var expander = new MTConnectMqttExpander("localhost");

var client = new MTConnectMqtt2Client("localhost");


client.DeviceReceived += async (sender, device) =>
{
    await expander.PublishDevice(device);

    Console.WriteLine($"Device Received : {device.Uuid}");
};
client.CurrentReceived += async (sender, document) =>
{
    await expander.PublishCurrent(document.GetObservations());
};
client.SampleReceived += async (sender, document) =>
{
    await expander.PublishSample(document.GetObservations());
};


//client.ObservationReceived += async (sender, observation) =>
//{
//    await expander.Publish(observation);
//};
client.AssetReceived += (sender, asset) =>
{
    Console.WriteLine($"Asset Received : {asset.AssetId}");
};

expander.Start();
client.Start();

Console.ReadLine();
