using MTConnect.Clients.Rest;

//var deviceName = "OKUMA-Lathe";
//var deviceName = "M12346";
var baseUrl = "127.0.0.1:5005";
//var baseUrl = "127.0.0.1:5005";
//var baseUrl = "localhost:5000";
//var baseUrl = "mtconnect.mazakcorp.com:5719";

var client = new MTConnectClient(baseUrl);


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
