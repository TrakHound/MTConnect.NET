// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Threading;
using System.Xml.Linq;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using MTConnect.Servers.Http;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    /// <summary>
    /// End-to-end workflow tests for the vendor-extension surface on
    /// <see cref="IConfiguration.VendorExtensions"/>. Boots an in-process
    /// <see cref="MTConnectAgentBroker"/> plus embedded
    /// <see cref="MTConnectHttpServer"/>, seeds a Device whose Linear
    /// component's Configuration carries a vendor-namespaced XElement, and
    /// asserts the emitted /probe response body contains the vendor element
    /// verbatim inside the <c>Configuration</c> envelope; a round-trip
    /// through the strongly-typed client model preserves the element's local
    /// name, namespace, attributes, and text content.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>XSD — <c>MTConnectDevices_2.7.xsd</c> line 8161 declares
    /// <c>ComponentConfigurationType</c> with
    /// <c>&lt;xs:element ref="AbstractConfiguration" minOccurs="0"
    /// maxOccurs="unbounded"/&gt;</c>; every standard child carries
    /// <c>substitutionGroup='AbstractConfiguration'</c>. Vendor extensions
    /// substitute into the same slot.</item>
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// UML class <c>Configuration</c>.</item>
    /// <item>Prose — <see href="https://docs.mtconnect.org/"/> Part 2
    /// (Devices) on Configuration and its extensibility.</item>
    /// </list>
    /// </remarks>
    [Trait("Category", "E2E")]
    public class ConfigurationVendorExtensionsHttpProbeWorkflowTests
    {
        // Ephemeral port range placed outside the existing MTAgentFixture
        // window plus other workflow fixtures.
        private static int s_nextPort = 6800 + (System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 800));

        private static int AllocatePort() => Interlocked.Increment(ref s_nextPort);

        /// <summary>A Device seeded with a vendor-namespaced XElement in its
        /// Configuration surfaces the element verbatim in the emitted /probe
        /// response body.</summary>
        [Fact]
        public void Probe_response_body_contains_vendor_extension_verbatim()
        {
            var port = AllocatePort();

            var device = BuildDeviceWithVendorExtension(
                XElement.Parse(
                    "<mycorp:Ext xmlns:mycorp=\"urn:mycorp:mtconnect\">"
                    + "<Foo attr=\"value\">child-text</Foo>"
                    + "</mycorp:Ext>"));

            var body = ProbeAndFetchBody(device, port);

            Assert.Contains("MTConnectDevices", body);
            Assert.Contains("mycorp:Ext", body);
            Assert.Contains("urn:mycorp:mtconnect", body);
            Assert.Contains("<Foo attr=\"value\">child-text</Foo>", body);
        }

        /// <summary>A Device seeded with a vendor XElement round-trips through
        /// the strongly-typed HTTP Probe client, arriving at the client with
        /// local name, namespace, attributes, and text preserved.</summary>
        [Fact]
        public void Probe_round_trips_vendor_extension_through_typed_client()
        {
            var port = AllocatePort();

            var original = XElement.Parse(
                "<mycorp:Ext xmlns:mycorp=\"urn:mycorp:mtconnect\" "
                + "vendorId=\"42\">"
                + "<Payload>hello</Payload>"
                + "</mycorp:Ext>");

            var device = BuildDeviceWithVendorExtension(original);

            var probeDevice = ProbeAndExtractDevice(device, port);
            var linear = FindLinear(probeDevice);

            Assert.NotNull(linear);
            Assert.NotNull(linear!.Configuration);
            Assert.NotNull(linear.Configuration!.VendorExtensions);

            var received = linear.Configuration.VendorExtensions.ToList();
            Assert.Single(received);

            var ext = received[0];
            Assert.Equal("Ext", ext.Name.LocalName);
            Assert.Equal("urn:mycorp:mtconnect", ext.Name.NamespaceName);
            Assert.Equal("42", ext.Attribute("vendorId")?.Value);
            Assert.Equal("hello", ext.Element("Payload")?.Value);
        }

        // ---------------- helpers ----------------

        private static IDevice BuildDeviceWithVendorExtension(XElement extension)
        {
            var device = new Device
            {
                Id = "d1",
                Name = "VendorExtDevice",
                Uuid = "00000000-0000-0000-0000-0000000000ff"
            };
            device.AddDataItem(new DataItem(DataItemCategory.EVENT, "AVAILABILITY", null, "avail"));

            var axes = new AxesComponent { Id = "ax", Name = "Axes" };
            var linear = new LinearComponent
            {
                Id = "x",
                Name = "X",
                Configuration = new Configuration
                {
                    VendorExtensions = new[] { extension }
                }
            };
            linear.AddDataItem(new DataItem(DataItemCategory.SAMPLE, "POSITION", null, "xpos") { Units = "MILLIMETER" });
            axes.AddComponent(linear);
            device.AddComponent(axes);

            return device;
        }

        private static string ProbeAndFetchBody(IDevice inputDevice, int port)
        {
            var agent = new MTConnectAgentBroker();
            agent.Start();
            try
            {
                agent.AddDevice(inputDevice);

                var serverConfig = new HttpServerConfiguration
                {
                    Port = port,
                    Server = "127.0.0.1"
                };
                using var server = new MTConnectHttpServer(serverConfig, agent);
                server.Start();
                try
                {
                    using var http = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new System.Uri($"http://127.0.0.1:{port}/"),
                        Timeout = System.TimeSpan.FromSeconds(15)
                    };

                    // Retry-loop for the listener-bind window under coverage
                    // instrumentation, matching the pattern in
                    // ConfigurationPolymorphicHttpProbeWorkflowTests.
                    string? body = null;
                    for (var attempt = 0; attempt < 20; attempt++)
                    {
                        try
                        {
                            var response = http.GetAsync("probe").GetAwaiter().GetResult();
                            if (response.IsSuccessStatusCode)
                            {
                                body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                                if (!string.IsNullOrEmpty(body)) break;
                            }
                        }
                        catch
                        {
                            // Ignore transient HTTP failures during bind window.
                        }
                        Thread.Sleep(100);
                    }

                    Assert.NotNull(body);
                    return body!;
                }
                finally
                {
                    server.Stop();
                }
            }
            finally
            {
                agent.Stop();
                Thread.Sleep(150);
            }
        }

        private static IDevice ProbeAndExtractDevice(IDevice inputDevice, int port)
        {
            var agent = new MTConnectAgentBroker();
            agent.Start();
            try
            {
                agent.AddDevice(inputDevice);

                var serverConfig = new HttpServerConfiguration { Port = port };
                using var server = new MTConnectHttpServer(serverConfig, agent);
                server.Start();
                try
                {
                    var client = new MTConnectHttpClient(
                        $"127.0.0.1:{port}",
                        inputDevice.Name);

                    IDevicesResponseDocument? probe = null;
                    for (var attempt = 0; attempt < 20; attempt++)
                    {
                        try
                        {
                            probe = client.GetProbe();
                            if (probe != null && !probe.Devices.IsNullOrEmpty()) break;
                        }
                        catch
                        {
                            // Ignore transient HTTP failures during bind window.
                        }
                        Thread.Sleep(100);
                    }

                    Assert.NotNull(probe);
                    Assert.NotEmpty(probe!.Devices);
                    return probe.Devices.First(d => d.Uuid == inputDevice.Uuid);
                }
                finally
                {
                    server.Stop();
                }
            }
            finally
            {
                agent.Stop();
                Thread.Sleep(150);
            }
        }

        private static LinearComponent? FindLinear(IDevice device)
        {
            return WalkComponents(device.Components).OfType<LinearComponent>().FirstOrDefault();
        }

        private static System.Collections.Generic.IEnumerable<IComponent> WalkComponents(
            System.Collections.Generic.IEnumerable<IComponent>? roots)
        {
            if (roots == null) yield break;
            foreach (var c in roots)
            {
                yield return c;
                foreach (var nested in WalkComponents(c.Components))
                {
                    yield return nested;
                }
            }
        }
    }
}
