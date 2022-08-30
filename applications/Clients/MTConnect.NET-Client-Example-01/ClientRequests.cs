using MTConnect.Clients.Rest;

//var deviceName = "OKUMA-Lathe";
//var deviceName = "M12346";

var agentUrl = "127.0.0.1:5005";
//var agentUrl = "127.0.0.1:5005";
//var agentUrl = "localhost:5000";
//var agentUrl = "mtconnect.mazakcorp.com:5719";

var client = new MTConnectClient(agentUrl);
//var client = new MTConnectClient(agentUrl, deviceName);


var probe = client.GetProbe();
if (probe != null)
{

}

Console.ReadLine();


var current = client.GetCurrent();
if (current != null)
{

}

Console.ReadLine();

var sample = client.GetSample();
if (sample != null)
{

}

Console.ReadLine();
