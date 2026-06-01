// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.Tests.Http.Clients
{
    // Drives MTConnectHttpSampleClient against the real embedded
    // MTConnectHttpServer started by AgentRunner, exercising the HTTP sample
    // request/response path end to end for XML and JSON, all devices, a single
    // device and a device path filter.
    /// <summary>Pins the behaviour expressed by the test name: sample.</summary>
    [TestFixture]
    public class Sample : HttpClientFixture
    {
        /// <summary>Pins the behaviour expressed by the test name: run xml.</summary>
        [Test]
        public void RunXml()
        {
            var client = new MTConnectHttpSampleClient(Hostname, Port, count: 1000, documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Sample response was not received");
            Assert.That(response.Streams.Count(), Is.EqualTo(ExpectedDocumentEntryCount), "XML Sample did not return all device streams");
        }

        /// <summary>Pins the behaviour expressed by the test name: run json.</summary>
        [Test]
        public void RunJson()
        {
            var client = new MTConnectHttpSampleClient(Hostname, Port, count: 1000, documentFormat: DocumentFormat.JSON);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "JSON Sample response was not received");
            Assert.That(response.Streams.Count(), Is.EqualTo(ExpectedDocumentEntryCount), "JSON Sample did not return all device streams");
        }

        /// <summary>Pins the behaviour expressed by the test name: run device xml.</summary>
        [Test]
        public void RunDeviceXml()
        {
            var client = new MTConnectHttpSampleClient(Hostname, Port, DeviceName, documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Sample response was not received");
            Assert.That(response.Streams.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"XML Sample did not return device {DeviceName}");
        }

        /// <summary>Pins the behaviour expressed by the test name: run device json.</summary>
        [Test]
        public void RunDeviceJson()
        {
            var client = new MTConnectHttpSampleClient(Hostname, Port, DeviceName, documentFormat: DocumentFormat.JSON);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "JSON Sample response was not received");
            Assert.That(response.Streams.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"JSON Sample did not return device {DeviceName}");
        }

        /// <summary>Pins the behaviour expressed by the test name: run device path xml.</summary>
        [Test]
        public void RunDevicePathXml()
        {
            var client = new MTConnectHttpSampleClient(Hostname, Port, DeviceName, path: "//DataItem[@type=\"AVAILABILITY\"]", documentFormat: DocumentFormat.XML);

            var response = client.Get();

            Assert.That(response, Is.Not.Null, "XML Sample path response was not received");
            Assert.That(response.Streams.FirstOrDefault(o => o.Name == DeviceName), Is.Not.Null, $"XML Sample path did not return device {DeviceName}");
        }
    }
}
