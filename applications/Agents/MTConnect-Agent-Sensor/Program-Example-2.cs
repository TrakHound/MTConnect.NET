// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using MTConnect.Streams.Xml;

namespace MTConnect.Applications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create MTConnect Agent
            var configuration = new MTConnectAgentConfiguration();
            var agent = new MTConnectAgent(configuration);
            agent.Version = new Version(1, 8);

            // Create Device
            var device = new Device();
            device.Id = "sensor-123";
            device.Name = "sensor";
            device.Uuid = "sensor-123";

            // Create DataItem
            var dataItem = new DataItem();
            dataItem.Category = DataItemCategory.SAMPLE;
            dataItem.Type = "TEMPERATURE";
            dataItem.Id = "temp";

            // Add DataItem to Device
            device.DataItems = new DataItem[] { dataItem };

            // Add Device to Agent
            agent.AddDevice(device);

            // Output Devices Response XML
            var deviceResponse = agent.GetDevices();
            var devicesXml = XmlDevicesResponseDocument.ToXml(deviceResponse, indent: true);
            Console.WriteLine(devicesXml);
            Console.WriteLine();
            Console.WriteLine("Press any key to output Streams..");
            Console.ReadLine();

            var rnd = new Random();

            while (true)
            {
                // Add Observation
                agent.AddObservation("sensor", "temp", rnd.Next());

                // Output Streams Response XML
                var streamsResponse = agent.GetDeviceStream("sensor");
                var streamsXml = XmlStreamsResponseDocument.ToXml(streamsResponse, indent: true);
                Console.WriteLine(streamsXml);
                Console.WriteLine();
                Console.WriteLine("Press any key to refresh Streams..");
                Console.ReadLine();
            }
        }
    }
}
