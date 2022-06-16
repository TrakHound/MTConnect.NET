using MTConnect.Shdr;

var client = new ShdrClient("localhost", 7878);
client.Connected += (s, e) =>
{
    Console.WriteLine("Connection Established");
};
client.ProtocolReceived += (s, line) =>
{
    Console.WriteLine(line);
};
client.Disconnected += (s, e) =>
{
    Console.WriteLine("Disconnected");
};
client.Start();

Console.ReadLine();

client.Stop();
