// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Threading;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using MTConnect.Servers.Http;
using Xunit;

// Disable cross-class parallelism for the IntegrationTests assembly. Both
// ConfigurationPolymorphicHttpProbeWorkflowTests and
// ClientAgentCommunicationTests boot real MTConnectAgentBroker + HTTP server
// instances; xUnit's default per-class parallelism races on broker static
// state and blanks out probe responses. Within-class tests already run
// sequentially per xUnit's default — this only disables inter-class.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace MTConnect.Tests.Integration.Workflows
{
    /// <summary>
    /// End-to-end workflow tests for the v2.7 polymorphic Configuration sub-
    /// element family. Each test boots an in-process MTConnect agent with a
    /// Device whose Configuration carries a polymorphic DataSet variant,
    /// fires an HTTP probe through MTConnectHttpClient, and asserts the
    /// returned envelope round-trips back to the expected I*DataSet
    /// narrowing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/></item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/></item>
    /// </list>
    /// Workflow catalog rows: W08 (Motion + AxisDataSet probe), W09
    /// (CoordinateSystem + OriginDataSet probe), W10 (Transformation +
    /// RotationDataSet probe).
    /// </para>
    /// <para>
    /// Cross-class test parallelism is disabled at the assembly level (see
    /// the <c>[assembly: CollectionBehavior(DisableTestParallelization = true)]</c>
    /// attribute above) so that this fixture's per-test broker + HTTP
    /// server boot does not race against
    /// <see cref="IntegrationTests.ClientAgentCommunicationTests"/>'s
    /// shared-broker fixture and blank out probe responses.
    /// </para>
    /// </remarks>
    public class ConfigurationPolymorphicHttpProbeWorkflowTests
    {
        // Each test allocates its own ephemeral port from a range outside
        // the existing MTAgentFixture's 5000-onwards window. The starting
        // value is randomized per process so concurrent test workers do
        // not collide on a fixed seed.
        private static int s_nextPort = 6000 + (System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 2000));

        private static int AllocatePort() => Interlocked.Increment(ref s_nextPort);

        // ---------------- W08: Motion + AxisDataSet ----------------

        [Fact]
        public void Probe_returns_AxisDataSet_round_tripped_through_HTTP()
        {
            var port = AllocatePort();

            var device = BuildDeviceWithLinear(linear =>
            {
                linear.Configuration = new Configuration
                {
                    Motion = new Motion
                    {
                        Id = "mx",
                        CoordinateSystemIdRef = "machine",
                        Type = MotionType.PRISMATIC,
                        Actuation = MotionActuationType.DIRECT,
                        Axis = new AxisDataSet { X = 1.0, Y = 2.0, Z = 3.0 }
                    }
                };
            });

            var (motion, _) = ProbeAndExtractMotion(device, port);

            Assert.NotNull(motion);
            Assert.IsAssignableFrom<IAxisDataSet>(motion!.Axis);
            var ds = (IAxisDataSet)motion.Axis;
            Assert.Equal(1.0, ds.X);
            Assert.Equal(2.0, ds.Y);
            Assert.Equal(3.0, ds.Z);
        }

        // ---------------- W09: CoordinateSystem + OriginDataSet ----------------

        [Fact]
        public void Probe_returns_OriginDataSet_round_tripped_through_HTTP()
        {
            var port = AllocatePort();

            var device = BuildDeviceWithLinear(linear =>
            {
                linear.Configuration = new Configuration
                {
                    CoordinateSystems = new[]
                    {
                        new CoordinateSystem
                        {
                            Id = "cs1",
                            Type = CoordinateSystemType.MACHINE,
                            Origin = new OriginDataSet { X = "10", Y = "20", Z = "30" }
                        }
                    }
                };
            });

            var probeDevice = ProbeAndExtractDevice(device, port);

            var linearComponent = FindLinear(probeDevice);
            Assert.NotNull(linearComponent);
            var cs = linearComponent!.Configuration?.CoordinateSystems?.FirstOrDefault();
            Assert.NotNull(cs);
            Assert.IsAssignableFrom<IOriginDataSet>(cs!.Origin);
            var ds = (IOriginDataSet)cs.Origin;
            Assert.Equal("10", ds.X);
            Assert.Equal("20", ds.Y);
            Assert.Equal("30", ds.Z);
        }

        // ---------------- W10: Transformation + RotationDataSet ----------------

        [Fact]
        public void Probe_returns_RotationDataSet_round_tripped_through_HTTP()
        {
            var port = AllocatePort();

            var device = BuildDeviceWithLinear(linear =>
            {
                linear.Configuration = new Configuration
                {
                    Motion = new Motion
                    {
                        Id = "mx",
                        CoordinateSystemIdRef = "machine",
                        Type = MotionType.PRISMATIC,
                        Actuation = MotionActuationType.DIRECT,
                        Transformation = new Transformation
                        {
                            Translation = new Translation { Value = "1 2 3" },
                            Rotation = new RotationDataSet { A = "10", B = "20", C = "30" }
                        }
                    }
                };
            });

            var (motion, _) = ProbeAndExtractMotion(device, port);

            Assert.NotNull(motion);
            Assert.NotNull(motion!.Transformation);
            Assert.IsAssignableFrom<IRotationDataSet>(motion.Transformation!.Rotation);
            var ds = (IRotationDataSet)motion.Transformation.Rotation;
            Assert.Equal("10", ds.A);
            Assert.Equal("20", ds.B);
            Assert.Equal("30", ds.C);
        }

        // ---------------- negative-path: simple Axis still narrows correctly ----------------

        [Fact]
        public void Probe_returns_simple_Axis_not_narrowed_to_DataSet()
        {
            // Negative path for the W08 workflow: when the Configuration
            // carries the simple Axis variant, the round-trip MUST NOT
            // narrow it to IAxisDataSet — pin against a regression that
            // greedily promotes every IAbstractAxis to its DataSet form.
            var port = AllocatePort();

            var device = BuildDeviceWithLinear(linear =>
            {
                linear.Configuration = new Configuration
                {
                    Motion = new Motion
                    {
                        Id = "mx",
                        CoordinateSystemIdRef = "machine",
                        Type = MotionType.PRISMATIC,
                        Actuation = MotionActuationType.DIRECT,
                        Axis = new Axis { Value = "1 2 3" }
                    }
                };
            });

            var (motion, _) = ProbeAndExtractMotion(device, port);

            Assert.NotNull(motion);
            Assert.IsAssignableFrom<IAxis>(motion!.Axis);
            Assert.False(motion.Axis is IAxisDataSet,
                "Simple Axis must not be narrowed to IAxisDataSet on round-trip");
            Assert.Equal("1 2 3", ((IAxis)motion.Axis).Value);
        }

        // ---------------- helpers ----------------

        private static IDevice BuildDeviceWithLinear(System.Action<LinearComponent> configure)
        {
            var device = new Device
            {
                Id = "d1",
                Name = "TestDevice",
                Uuid = "00000000-0000-0000-0000-000000000001"
            };
            device.AddDataItem(new DataItem(DataItemCategory.EVENT, "AVAILABILITY", null, "avail"));

            var axes = new AxesComponent { Id = "ax", Name = "Axes" };
            var linear = new LinearComponent { Id = "x", Name = "X" };
            linear.AddDataItem(new DataItem(DataItemCategory.SAMPLE, "POSITION", null, "xpos") { Units = "MILLIMETER" });
            configure(linear);
            axes.AddComponent(linear);
            device.AddComponent(axes);

            return device;
        }

        private static (IMotion? motion, IDevice device) ProbeAndExtractMotion(IDevice inputDevice, int port)
        {
            var probeDevice = ProbeAndExtractDevice(inputDevice, port);
            var linear = FindLinear(probeDevice);
            return (linear?.Configuration?.Motion, probeDevice);
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

                    // server.Start() returns before the HTTP listener is
                    // actually bound and ready to accept connections. Retry
                    // the probe up to ~2 s so a slow socket bind on a busy
                    // host (CI under coverage instrumentation) does not
                    // surface as a null-probe assertion failure.
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
                            // ignore transient HTTP failures during the
                            // listener-bind window
                        }
                        Thread.Sleep(100);
                    }

                    Assert.NotNull(probe);
                    Assert.NotEmpty(probe.Devices);
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

                // MTConnectAgentBroker.Stop() is fire-and-forget — the
                // broker's internal cleanup races with the next test's
                // boot. ClientAgentCommunicationTests' Dispose carries the
                // same caveat. Pause so cross-class tests that follow
                // this fixture (e.g. ClientAgentCommunicationTests) can
                // bind the same listener-loop state cleanly.
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
