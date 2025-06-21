using MTConnect.Shdr;

namespace MTConnect.Clients.SHDR
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Hostname:");
            var hostname = Console.ReadLine();

            Console.WriteLine("Enter Port:");
            var port = Console.ReadLine().ToInt();

            Console.WriteLine($"Connecting to ({hostname}:{port})..");
            var client = new ShdrClient(hostname, port);
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
