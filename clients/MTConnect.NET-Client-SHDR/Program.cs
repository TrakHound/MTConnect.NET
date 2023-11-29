using MTConnect.Shdr;

namespace MTConnect.Clients.SHDR
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
