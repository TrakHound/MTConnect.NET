namespace MTConnect.Clients.HTTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DocumentClient();
            //EntityClient();

            Console.ReadLine();
        }

        static void DocumentClient()
        {
            //var client = new MTConnectHttpClient("http://mtconnect.mazakcorp.com/", 5719);
            var client = new MTConnectHttpClient("localhost", 5000);
            //var client = new MTConnectHttpClient("localhost", 5001);
            client.Interval = 100;
            //client.Heartbeat = 0;
            client.ClientStarted += (s, args) => { Console.WriteLine("Client Started"); };
            client.ClientStopped += (s, args) => { Console.WriteLine("Client Stopped"); };

            client.ProbeReceived += (s, response) =>
            {
                foreach (var device in response.Devices) Console.WriteLine($"Device Received : {device.Uuid} : {device.Name}");
            };

            client.CurrentReceived += (s, response) =>
            {
                foreach (var stream in response.Streams)
                {
                    foreach (var componentStream in stream.ComponentStreams)
                    {
                        foreach (var observation in componentStream.Observations)
                        {
                            Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(";", observation.Values.Select(o => o.Value))}");
                        }
                    }
                }
            };

            client.SampleReceived += (s, response) =>
            {
                foreach (var stream in response.Streams)
                {
                    foreach (var componentStream in stream.ComponentStreams)
                    {
                        foreach (var observation in componentStream.Observations)
                        {
                            Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(";", observation.Values.Select(o => o.Value))}");
                        }
                    }
                }
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
