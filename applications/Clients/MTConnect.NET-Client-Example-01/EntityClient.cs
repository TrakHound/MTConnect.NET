using MTConnect.Clients;

//var deviceName = "OKUMA-Lathe";
//var deviceName = "M12346";

//var agentUrl = "localhost:5006";
//var agentUrl = "localhost:5005";
//var agentUrl = "http://localhost:5001";
//var agentUrl = "http://DESKTOP-HV74M4N:5001";
//var agentUrl = "https://localhost:5002";
//var agentUrl = "https://DESKTOP-HV74M4N:5002";
var agentUrl = "localhost:5000";
//var agentUrl = "192.168.1.136:5000";
//var agentUrl = "mtconnect.mazakcorp.com:5719";
//var agentUrl = "https://trakhound.com";


for (int i = 0; i < 1; i++)
{
    var client = new MTConnectMqttClient("localhost", interval: 1000);
    //var client = new MTConnectHttpClient(agentUrl);
    //var client = new MTConnectClient(agentUrl, deviceName);
    //client.Interval = 100;
    //client.Heartbeat = 10000;
    //client.ContentEncodings = null;
    //client.ContentType = null;
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
    //client.StartFromBuffer();
    //client.Start("//*[@type=\"PATH_POSITION\"]");
}

Console.ReadLine();
