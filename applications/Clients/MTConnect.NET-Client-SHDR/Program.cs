using MTConnect.Shdr;

var client = new ShdrClient("localhost", 7878);
client.AdapterConnected += (s, e) =>
{
    Console.WriteLine("Connection Established");
};
client.ProtocolReceived += (s, line) =>
{
    Console.WriteLine(line);
};
client.AdapterDisconnected += (s, e) =>
{
    Console.WriteLine("Disconnected");
};
client.Start();

Console.ReadLine();

client.Stop();
