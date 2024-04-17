using MTConnect.Configurations;
using MTConnect.Formatters;
using MTConnect.Observations;

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
            var config = new MTConnectMqttClientConfiguration();
            config.Server = "localhost";
            config.Port = 1883;
            config.TopicPrefix = "MTConnect/Document";

            var client = new MTConnectMqttClient(config);
            client.ClientStarted += (s, args) => { Console.WriteLine("Client Started"); };
            client.ClientStopped += (s, args) => { Console.WriteLine("Client Stopped"); };

            //client.MessageReceived += (topic, payload) => Console.WriteLine($"Message Received : {topic} : {payload.Length}");
            client.DeviceReceived += (topic, device) => Console.WriteLine($"Device Received : {device.Uuid} : {device.Name}");
            client.ObservationReceived += (topic, observation) => Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(";", observation.Values.Select(o => o.Value))}");

            //client.ProbeReceived += (s, response) =>
            //{
            //    foreach (var device in response.Devices) Console.WriteLine($"Device Received : {device.Uuid} : {device.Name}");
            //};

            //client.CurrentReceived += (s, response) =>
            //{
            //    foreach (var stream in response.Streams)
            //    {
            //        foreach (var componentStream in stream.ComponentStreams)
            //        {
            //            foreach (var observation in componentStream.Observations)
            //            {
            //                Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(";", observation.Values.Select(o => o.Value))}");
            //            }
            //        }
            //    }
            //};

            //client.SampleReceived += (s, response) =>
            //{
            //    foreach (var stream in response.Streams)
            //    {
            //        foreach (var componentStream in stream.ComponentStreams)
            //        {
            //            foreach (var observation in componentStream.Observations)
            //            {
            //                Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(";", observation.Values.Select(o => o.Value))}");
            //            }
            //        }
            //    }
            //};

            //client.AssetsReceived += (s, response) =>
            //{
            //    foreach (var asset in response.Assets)
            //    {
            //        var result = EntityFormatter.Format("XML", asset);
            //        //if (result.Success) Console.WriteLine(System.Text.Encoding.UTF8.GetString(result.Content));
            //    }
            //};

            client.Start();
        }

        static void EntityClient()
        {
            var client = new MTConnectMqttClient("localhost", 1883);
            client.ClientStarted += (s, args) => { Console.WriteLine("Client Started"); };
            client.ClientStopped += (s, args) => { Console.WriteLine("Client Stopped"); };

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
