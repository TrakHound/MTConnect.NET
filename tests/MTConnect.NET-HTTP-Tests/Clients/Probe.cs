// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.Tests.Http.Clients
{
    // Drives MTConnectHttpProbeClient against the real embedded
    // MTConnectHttpServer started by AgentRunner, exercising the HTTP probe
    // request/response path end to end for XML and JSON, all devices and a
    // single device.
    [TestFixture]
    public class Probe : HttpClientFixture
    {
        [Test]
        public void RunXml()
        {
            var client = new MTConnectHttpProbeClient(Hostname, Port, documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Probe response was not received");
            Assert.That(response.Devices.Count(), Is.EqualTo(ExpectedDocumentEntryCount), "XML Probe did not return all devices");
        }

        [Test]
        public void RunJson()
        {
            var client = new MTConnectHttpProbeClient(Hostname, Port, documentFormat: DocumentFormat.JSON);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "JSON Probe response was not received");
            Assert.That(response.Devices.Count(), Is.EqualTo(ExpectedDocumentEntryCount), "JSON Probe did not return all devices");
        }

        [Test]
        public void RunDeviceXml()
        {
            var client = new MTConnectHttpProbeClient(Hostname, Port, DeviceName, documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Probe response was not received");
            Assert.That(response.Devices.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"XML Probe did not return device {DeviceName}");
        }

        [Test]
        public void RunDeviceJson()
        {
            var client = new MTConnectHttpProbeClient(Hostname, Port, DeviceName, documentFormat: DocumentFormat.JSON);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "JSON Probe response was not received");
            Assert.That(response.Devices.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"JSON Probe did not return device {DeviceName}");
        }
    }
}
