using System;
using System.IO;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices
{
    public class DeviceFiles
    {
        private const string DevicesDirectory = "Device-Files";
        private const string DevicesFilename = "devices-okuma-lathe.xml";


        [SetUp]
        public void Setup() { }

        [Test]
        public void LoadFile()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DevicesDirectory);
            var path = Path.Combine(dir, DevicesFilename);

            // Read 'devices.xml' file
            var devices = DeviceConfiguration.FromFile(path, DocumentFormat.XML);
            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    Console.WriteLine($"{device.Name} : {device.Id} : {device.Uuid}");
                }

                Assert.Pass($"XML Devices file Read Successfully : {path}");
            }
            else
            {
                Assert.Fail($"XML Devices file could not be read : {path}");
            }
        }

        [Test]
        public void LoadFiles()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DevicesDirectory);

            // Read 'devices.xml' file
            var devices = DeviceConfiguration.FromFiles(dir, DocumentFormat.XML);
            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    Console.WriteLine($"{device.Name} : {device.Id} : {device.Uuid}");
                }

                Assert.Pass($"XML Devices files Read Successfully : {dir}");
            }
            else
            {
                Assert.Fail($"XML Devices files could not be read : {dir}");
            }
        }
    }
}