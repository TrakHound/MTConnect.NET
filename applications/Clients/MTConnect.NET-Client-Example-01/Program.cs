using MTConnect.Clients.Rest;

//var deviceName = "OKUMA-Lathe";
//var deviceName = "M12346";
//var baseUrl = "localhost:5005";
var baseUrl = "mtconnect.mazakcorp.com:5719";

var client = new MTConnectClient(baseUrl);

Console.ReadLine();

//var doc = await client.GetProbeAsync();
//if (doc != null)
//{

//}

var doc = await client.GetSampleAsync(path: "//*[@type=\"LOAD\"]");
if (doc != null)
{

}

Console.ReadLine();
