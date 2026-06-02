// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.Tests.Http.Clients
{
    // Drives MTConnectHttpCurrentClient against the real embedded
    // MTConnectHttpServer started by AgentRunner, exercising the HTTP current
    // request/response path end to end for XML and JSON, all devices, a single
    // device and a device path filter.
    /// <summary>Pins the behaviour expressed by the test name: current.</summary>
    [TestFixture]
    public class Current : HttpClientFixture
    {
        /// <summary>Pins the behaviour expressed by the test name: run xml.</summary>
        [Test]
        public void RunXml()
        {
            var client = new MTConnectHttpCurrentClient(Hostname, Port, documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Current response was not received");
            Assert.That(response.Streams.Count(), Is.EqualTo(ExpectedDocumentEntryCount), "XML Current did not return all device streams");
        }

        /// <summary>Pins the behaviour expressed by the test name: run json.</summary>
        [Test]
        public void RunJson()
        {
            var client = new MTConnectHttpCurrentClient(Hostname, Port, documentFormat: DocumentFormat.JSON);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "JSON Current response was not received");
            Assert.That(response.Streams.Count(), Is.EqualTo(ExpectedDocumentEntryCount), "JSON Current did not return all device streams");
        }

        /// <summary>Pins the behaviour expressed by the test name: run device xml.</summary>
        [Test]
        public void RunDeviceXml()
        {
            var client = new MTConnectHttpCurrentClient(Hostname, Port, DeviceName, documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Current response was not received");
            Assert.That(response.Streams.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"XML Current did not return device {DeviceName}");
        }

        /// <summary>Pins the behaviour expressed by the test name: run device json.</summary>
        [Test]
        public void RunDeviceJson()
        {
            var client = new MTConnectHttpCurrentClient(Hostname, Port, DeviceName, documentFormat: DocumentFormat.JSON);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "JSON Current response was not received");
            Assert.That(response.Streams.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"JSON Current did not return device {DeviceName}");
        }

        /// <summary>Pins the behaviour expressed by the test name: run device path xml.</summary>
        [Test]
        public void RunDevicePathXml()
        {
            var client = new MTConnectHttpCurrentClient(Hostname, Port, DeviceName, path: "//DataItem[@type=\"AVAILABILITY\"]", documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Current path response was not received");
            Assert.That(response.Streams.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"XML Current path did not return device {DeviceName}");
        }
    }
}
