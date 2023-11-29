namespace MTConnect.Clients.HTTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DocumentClient();
            EntityClient();

            Console.ReadLine();
        }

        static void DocumentClient()
        {
            var client = new MTConnectHttpClient("localhost", 5000);

            client.ProbeReceived += (s, response) =>
            {
                Console.WriteLine(response.Devices.Count());
            };

            client.CurrentReceived += (s, response) =>
            {
                Console.WriteLine(response.Streams.Count());
            };

            client.SampleReceived += (s, response) =>
            {
                Console.WriteLine(response.Streams.Count());
            };

            client.AssetsReceived += (s, response) =>
            {
                Console.WriteLine(response.Assets.Count());
            };

            client.Start();
        }

        static void EntityClient()
        {
            var client = new MTConnectHttpClient("localhost", 5000);

            client.DeviceReceived += (s, device) =>
            {
                Console.WriteLine(device.Uuid);
            };

            client.ObservationReceived += (s, observation) =>
            {
                Console.WriteLine(observation.Uuid);
            };

            client.AssetReceived += (s, asset) =>
            {
                Console.WriteLine(asset.Uuid);
            };

            client.Start();
        }
    }
}
