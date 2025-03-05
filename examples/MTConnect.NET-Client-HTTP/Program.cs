﻿using MTConnect.Formatters;
using MTConnect.Input;
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
            var client = new MTConnectHttpClient("localhost", 5000);
            //var client = new MTConnectHttpClient("http://mtconnect.mazakcorp.com/", 5719);
            client.Interval = 100;
            client.ClientStarted += (s, args) => { Console.WriteLine("Client Started"); };
            client.ClientStopped += (s, args) => { Console.WriteLine("Client Stopped"); };
            //client.FormatError += (s, args) => { Console.WriteLine($"Format Error : {args.ContentType.Name} : {args.Messages?.FirstOrDefault()}"); };

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

                            var validationResult = observation.Validate();
                            Console.WriteLine($"Observation Validation : {observation.DataItemId} : {validationResult.IsValid} : {validationResult.Message}");
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

                            var validationResult = observation.Validate();
                            Console.WriteLine($"Observation Validation : {observation.DataItemId} : {validationResult.IsValid} : {validationResult.Message}");
                        }
                    }
                }
            };

            client.AssetsReceived += (s, response) =>
            {
                foreach (var asset in response.Assets)
                {
                    Console.WriteLine($"Asset Received : {asset.AssetId}");
                }
            };

            client.StartFromBuffer();
            //client.Start();
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
