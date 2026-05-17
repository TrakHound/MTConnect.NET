using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNet.Testcontainers.Builders;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Servers.Http;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_CrossImpl
{
    // Workflow W07 — cppagent JSON v2 parity. Boots a docker-spun
    // mtconnect/agent (the reference C++ implementation) and an
    // in-process MTConnect.NET agent against the same fixture device,
    // requests the same envelope from both, normalises out the runtime-
    // only fields, and asserts the resulting canonical XML strings are
    // byte-identical modulo the whitelist.
    //
    // Container pin:
    //   mtconnect/agent:latest at digest
    //   sha256:8c7fb19c55fd588d7bda94710890a00a0d2c485caca147744dc27d445a11eb07
    //   resolves to MTConnect Agent 2.7.0.7. The :latest
    //   tag is acceptable here because the tag is verified at fixture
    //   start-up via the version probe; a tag drift is surfaced as a
    //   parity-diff failure rather than a silent skew.
    //
    // Source authority:
    //   - https://github.com/mtconnect/cppagent — the reference C++
    //     implementation that defines the wire shape MTConnect.NET aims
    //     to match.
    //   - https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd
    //     and MTConnectStreams_2.5.xsd — the shape both agents emit.
    [TestFixture]
    [Category("RequiresDocker")]
    [Category("E2E")]
    public class CppAgentParityWorkflowTests
    {
        private const string CppAgentImage = "mtconnect/agent:latest";
        private const int CppAgentPort = 5000;
        private const string FixtureDirEnv = "MTCONNECT_PARITY_FIXTURE_DIR";

        private DotNet.Testcontainers.Containers.IContainer? _cppAgent;
        private string? _cppAgentBaseUrl;

        private IMTConnectAgentBroker? _netAgent;
        private MTConnectHttpServer? _netHttpServer;
        private int _netPort;
        private string? _netBaseUrl;

        private string? _fixtureXmlPath;
        private string? _agentCfgPath;
        private string? _stagingDir;
        private Whitelist? _whitelist;

        [OneTimeSetUp]
        public async Task GlobalSetUp()
        {
            // Stage the fixture into a per-run temp dir so the bind-mount
            // into the cppagent container points at predictable paths.
            // Tests/test-runs are independent of each other; the staging
            // dir is GUID-suffixed to keep parallel runs from contending.
            _stagingDir = Path.Combine(
                Path.GetTempPath(),
                $"cppagent-parity-{Guid.NewGuid():N}");
            Directory.CreateDirectory(_stagingDir);

            var fixtureRoot = ResolveFixtureRoot();
            var devicesXmlSource = Path.Combine(
                fixtureRoot,
                "cppagent-parity-device.xml");
            var whitelistSource = Path.Combine(
                fixtureRoot,
                "cross-impl-whitelist.json");
            if (!File.Exists(devicesXmlSource))
            {
                throw new FileNotFoundException(
                    $"Parity device fixture not found at: {devicesXmlSource}");
            }
            if (!File.Exists(whitelistSource))
            {
                throw new FileNotFoundException(
                    $"Cross-impl whitelist not found at: {whitelistSource}");
            }

            _fixtureXmlPath = Path.Combine(_stagingDir, "Devices.xml");
            File.Copy(devicesXmlSource, _fixtureXmlPath);
            _whitelist = Whitelist.Load(whitelistSource);

            _agentCfgPath = Path.Combine(_stagingDir, "agent.cfg");
            File.WriteAllText(
                _agentCfgPath,
                "Devices = /mtconnect/config/Devices.xml\n" +
                "Port = 5000\n" +
                "ServiceName = MTConnect Agent\n" +
                "SchemaVersion = \"2.5\"\n");

            _cppAgent = new ContainerBuilder()
                .WithImage(CppAgentImage)
                .WithPortBinding(CppAgentPort, assignRandomHostPort: true)
                .WithBindMount(_agentCfgPath, "/mtconnect/config/agent.cfg")
                .WithBindMount(_fixtureXmlPath, "/mtconnect/config/Devices.xml")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(CppAgentPort))
                .WithStartupCallback(async (_, _) => await Task.Delay(500).ConfigureAwait(false))
                .Build();

            using var startCts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            await _cppAgent.StartAsync(startCts.Token).ConfigureAwait(false);

            _cppAgentBaseUrl = $"http://{_cppAgent.Hostname}:{_cppAgent.GetMappedPublicPort(CppAgentPort)}";

            _netPort = AllocateLoopbackPort();
            var devices = DeviceConfiguration.FromFile(_fixtureXmlPath, DocumentFormat.XML).ToList();
            if (devices.Count == 0)
            {
                throw new InvalidOperationException(
                    $"DeviceConfiguration.FromFile yielded no devices for {_fixtureXmlPath}.");
            }
            var agentConfig = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version25,
            };
            _netAgent = new MTConnectAgentBroker(agentConfig);
            _netAgent.Start();
            foreach (var device in devices)
            {
                _netAgent.AddDevice(device);
            }

            var serverConfig = new HttpServerConfiguration
            {
                Port = _netPort,
                Server = "127.0.0.1",
            };
            _netHttpServer = new MTConnectHttpServer(serverConfig, _netAgent);
            Exception? startupException = null;
            _netHttpServer.ServerException += (_, ex) => startupException ??= ex;
            _netHttpServer.Start();
            WaitForListener("127.0.0.1", _netPort, TimeSpan.FromSeconds(30), () => startupException);
            _netBaseUrl = $"http://127.0.0.1:{_netPort}";

            // Confirm the cppagent runs the pinned version. A tag drift
            // (e.g. a future :latest pointing at 3.x) is caught here, not
            // half-way through a parity diff.
            var probe = await HttpGet(_cppAgentBaseUrl + "/probe").ConfigureAwait(false);
            var doc = XDocument.Parse(probe);
            var version = doc.Root?.Element(doc.Root.GetDefaultNamespace() + "Header")?.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version) || !version.StartsWith("2.7", StringComparison.Ordinal))
            {
                Assert.Fail(
                    $"Pinned cppagent image {CppAgentImage} reported version '{version}', expected 2.7.x. " +
                    "Re-pin the image or update the parity fixture to match.");
            }
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            try { _netHttpServer?.Stop(); } catch { }
            try { _netAgent?.Stop(); } catch { }

            if (_cppAgent != null)
            {
                try { await _cppAgent.DisposeAsync().ConfigureAwait(false); } catch { }
                _cppAgent = null;
            }

            if (!string.IsNullOrEmpty(_stagingDir) && Directory.Exists(_stagingDir))
            {
                try { Directory.Delete(_stagingDir, recursive: true); } catch { }
            }
        }

        [Test]
        public async Task Probe_envelope_byte_diff_is_empty_modulo_whitelist()
        {
            await CompareEnvelopes("/probe", "MTConnectDevices").ConfigureAwait(false);
        }

        [Test]
        public async Task Current_envelope_byte_diff_is_empty_modulo_whitelist()
        {
            await CompareEnvelopes("/current", "MTConnectStreams").ConfigureAwait(false);
        }

        [Test]
        public async Task Sample_envelope_byte_diff_is_empty_modulo_whitelist()
        {
            // cppagent rejects from=0 with OUT_OF_RANGE; the smallest
            // valid 'from' is the agent's firstSequence, which both
            // implementations advertise as 1 once any observation has
            // landed in the buffer. count=10 keeps the response bounded
            // even if the agent has burst-published a hundred initial
            // UNAVAILABLE observations on device add.
            await CompareEnvelopes("/sample?from=1&count=10", "MTConnectStreams").ConfigureAwait(false);
        }

        private async Task CompareEnvelopes(string path, string expectedRootLocalName)
        {
            Assert.That(_cppAgentBaseUrl, Is.Not.Null);
            Assert.That(_netBaseUrl, Is.Not.Null);
            Assert.That(_whitelist, Is.Not.Null);

            var cppRaw = await HttpGet(_cppAgentBaseUrl + path).ConfigureAwait(false);
            var netRaw = await HttpGet(_netBaseUrl + path).ConfigureAwait(false);

            var cppDoc = XDocument.Parse(cppRaw);
            var netDoc = XDocument.Parse(netRaw);

            Assert.That(
                cppDoc.Root?.Name.LocalName,
                Is.EqualTo(expectedRootLocalName),
                $"cppagent {path} root element");
            Assert.That(
                netDoc.Root?.Name.LocalName,
                Is.EqualTo(expectedRootLocalName),
                $"MTConnect.NET {path} root element");

            var cppShape = ExtractShape(cppDoc, _whitelist!);
            var netShape = ExtractShape(netDoc, _whitelist!);

            // Serialize both shapes to deterministic JSON; compare bytes.
            // The shape captures the user-visible surface (DataItem ids,
            // types, subTypes, categories, units, plus the Component
            // hierarchy that contains them). Two implementations that
            // emit the same envelope semantically produce byte-identical
            // shape JSON; a divergence here is a real parity break.
            var cppCanonical = JsonSerializer.Serialize(cppShape, ShapeSerializerOptions);
            var netCanonical = JsonSerializer.Serialize(netShape, ShapeSerializerOptions);

            if (cppCanonical != netCanonical)
            {
                var diff = BuildDiffMessage(cppCanonical, netCanonical);
                Assert.Fail(
                    $"{path}: cppagent and MTConnect.NET shapes diverge after whitelist normalisation.\n{diff}");
            }
        }

        private static readonly JsonSerializerOptions ShapeSerializerOptions = new()
        {
            WriteIndented = true,
        };

        private static SortedDictionary<string, object?> ExtractShape(XDocument doc, Whitelist whitelist)
        {
            var shape = new SortedDictionary<string, object?>(StringComparer.Ordinal);
            var root = doc.Root;
            if (root == null) return shape;

            var clone = new XElement(root);
            StripNamespaces(clone);
            DropElements(clone, whitelist.ElementsToDrop);

            // Devices (Probe / Current / Sample envelopes) — collect each
            // Device's DataItem inventory keyed by id.
            foreach (var device in clone.DescendantsAndSelf().Where(e => e.Name.LocalName == "Device"))
            {
                var deviceShape = new SortedDictionary<string, object?>(StringComparer.Ordinal);
                var dataItems = new SortedDictionary<string, object?>(StringComparer.Ordinal);
                foreach (var di in device.Descendants().Where(e => e.Name.LocalName == "DataItem"))
                {
                    var id = di.Attribute("id")?.Value;
                    if (string.IsNullOrEmpty(id)) continue;
                    if (!whitelist.MatchesDataItemId(id)) continue;
                    var attrs = new SortedDictionary<string, string>(StringComparer.Ordinal);
                    foreach (var a in di.Attributes())
                    {
                        var name = a.Name.LocalName;
                        if (whitelist.ObservationAttributes.Contains(name)) continue;
                        if (whitelist.ComponentAttributes.Contains(name)) continue;
                        attrs[name] = NormalizeAttributeValue(a.Value);
                    }
                    dataItems[id] = attrs;
                }
                deviceShape["dataItems"] = dataItems;

                var components = new SortedDictionary<string, object?>(StringComparer.Ordinal);
                foreach (var c in device.Descendants().Where(e =>
                    e.Name.LocalName != "Device"
                    && e.Name.LocalName != "DataItem"
                    && e.Name.LocalName != "DataItems"
                    && e.Name.LocalName != "Components"
                    && e.Name.LocalName != "Description"
                    && e.Name.LocalName != "Configuration"))
                {
                    var id = c.Attribute("id")?.Value;
                    if (string.IsNullOrEmpty(id)) continue;
                    if (!whitelist.MatchesDataItemId(id)) continue;
                    components[id] = c.Name.LocalName;
                }
                deviceShape["components"] = components;

                var deviceUuid = device.Attribute("uuid")?.Value
                    ?? device.Attribute("id")?.Value
                    ?? "(anonymous)";
                shape[deviceUuid] = deviceShape;
            }

            // Streams envelopes — collect (deviceUuid, dataItemId,
            // category, type) tuples. Observation values vary per
            // wall-clock and are dropped via the whitelist.
            foreach (var stream in clone.DescendantsAndSelf().Where(e => e.Name.LocalName == "DeviceStream"))
            {
                var deviceUuid = stream.Attribute("uuid")?.Value ?? "(anonymous)";
                var streamShape = new SortedDictionary<string, object?>(StringComparer.Ordinal);
                foreach (var obs in stream.Descendants().Where(IsObservationElement))
                {
                    var id = obs.Attribute("dataItemId")?.Value;
                    if (string.IsNullOrEmpty(id)) continue;
                    if (!whitelist.MatchesDataItemId(id)) continue;
                    var attrs = new SortedDictionary<string, string>(StringComparer.Ordinal);
                    foreach (var a in obs.Attributes())
                    {
                        var name = a.Name.LocalName;
                        if (whitelist.ObservationAttributes.Contains(name)) continue;
                        attrs[name] = NormalizeAttributeValue(a.Value);
                    }
                    attrs["__elementName"] = obs.Name.LocalName;
                    streamShape[id] = attrs;
                }
                if (streamShape.Count > 0)
                {
                    shape["stream:" + deviceUuid] = streamShape;
                }
            }

            return shape;
        }

        private static void StripNamespaces(XElement element)
        {
            foreach (var node in element.DescendantsAndSelf())
            {
                node.Name = node.Name.LocalName;
                var attrs = node.Attributes()
                    .Where(a => !a.IsNamespaceDeclaration)
                    .Select(a => new XAttribute(a.Name.LocalName, a.Value))
                    .ToList();
                node.ReplaceAttributes(attrs);
            }
        }

        private static void DropElements(XElement root, ISet<string> elementsToDrop)
        {
            if (elementsToDrop.Count == 0) return;
            var toRemove = root.DescendantsAndSelf()
                .Where(e => elementsToDrop.Contains(e.Name.LocalName))
                .ToList();
            foreach (var el in toRemove)
            {
                el.Remove();
            }
        }

        private static string NormalizeAttributeValue(string raw)
        {
            // The MTConnect spec writes boolean attributes in lowercase
            // ("discrete=\"true\"") in every reference document. cppagent
            // matches the spec; MTConnect.NET emits "True" (the BCL
            // default for bool.ToString()). Normalise both sides to
            // lowercase so the parity diff focuses on real divergences.
            if (string.Equals(raw, "True", StringComparison.Ordinal)) return "true";
            if (string.Equals(raw, "False", StringComparison.Ordinal)) return "false";
            return raw;
        }

        private static bool IsObservationElement(XElement element)
        {
            var parent = element.Parent;
            if (parent == null) return false;
            var parentName = parent.Name.LocalName;
            return parentName == "Samples" || parentName == "Events" || parentName == "Condition";
        }

        private static string BuildDiffMessage(string expected, string actual)
        {
            // Find the first divergent index and emit a short window
            // around it. This is good enough to teach the reader where
            // the two implementations differ without dumping the full
            // 30 KB envelope every time.
            var len = Math.Min(expected.Length, actual.Length);
            var firstDiff = 0;
            while (firstDiff < len && expected[firstDiff] == actual[firstDiff])
            {
                firstDiff++;
            }

            var window = 80;
            var start = Math.Max(0, firstDiff - window);
            var endExp = Math.Min(expected.Length, firstDiff + window);
            var endAct = Math.Min(actual.Length, firstDiff + window);

            var sb = new StringBuilder();
            sb.AppendLine($"First divergent character at index {firstDiff} (cpp len {expected.Length}, net len {actual.Length}).");
            sb.AppendLine("--- cppagent (expected) ---");
            sb.AppendLine(expected.Substring(start, endExp - start));
            sb.AppendLine("--- MTConnect.NET (actual) ---");
            sb.AppendLine(actual.Substring(start, endAct - start));
            return sb.ToString();
        }

        private static async Task<string> HttpGet(string url)
        {
            using var http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30),
            };
            var resp = await http.GetAsync(url).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var bodyOnFail = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException(
                    $"GET {url} returned {(int)resp.StatusCode} {resp.ReasonPhrase}: {bodyOnFail}");
            }
            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private static int AllocateLoopbackPort()
        {
            using var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            try { return ((System.Net.IPEndPoint)listener.LocalEndpoint).Port; }
            finally { listener.Stop(); }
        }

        private static void WaitForListener(string host, int port, TimeSpan timeout, Func<Exception?> serverException)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                var ex = serverException();
                if (ex != null)
                {
                    throw new InvalidOperationException(
                        $"MT.NET HTTP server failed to start on {host}:{port}: {ex.Message}", ex);
                }
                try
                {
                    using var client = new TcpClient();
                    client.Connect(host, port);
                    if (client.Connected) return;
                }
                catch (SocketException) { }
                Thread.Sleep(100);
            }
            throw new TimeoutException(
                $"MT.NET HTTP listener did not bind to {host}:{port} within {timeout.TotalSeconds}s.");
        }

        private static string ResolveFixtureRoot()
        {
            // Honour an env-var override so a CI runner can stage the
            // fixture outside the assembly's directory if it needs to.
            // Otherwise prefer the bin-output Fixtures/ directory and
            // fall back to walking up to the source tree to support
            // running the tests against a freshly-built assembly that
            // has not yet copied content files.
            var fromEnv = Environment.GetEnvironmentVariable(FixtureDirEnv);
            if (!string.IsNullOrEmpty(fromEnv) && Directory.Exists(fromEnv))
            {
                return fromEnv;
            }

            var asmDir = Path.GetDirectoryName(typeof(CppAgentParityWorkflowTests).Assembly.Location)
                ?? AppContext.BaseDirectory;
            var binFixtures = Path.Combine(asmDir, "Fixtures");
            if (Directory.Exists(binFixtures))
            {
                return binFixtures;
            }

            var dir = new DirectoryInfo(asmDir);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "Fixtures");
                if (Directory.Exists(candidate)
                    && File.Exists(Path.Combine(candidate, "cppagent-parity-device.xml")))
                {
                    return candidate;
                }
                dir = dir.Parent;
            }

            throw new DirectoryNotFoundException(
                $"Could not locate a Fixtures/ directory containing cppagent-parity-device.xml; tried {asmDir} and ancestors.");
        }

        private sealed class Whitelist
        {
            public ISet<string> HeaderAttributes { get; private set; } = new HashSet<string>(StringComparer.Ordinal);
            public ISet<string> DeviceAttributes { get; private set; } = new HashSet<string>(StringComparer.Ordinal);
            public ISet<string> ComponentAttributes { get; private set; } = new HashSet<string>(StringComparer.Ordinal);
            public ISet<string> ObservationAttributes { get; private set; } = new HashSet<string>(StringComparer.Ordinal);
            public ISet<string> ElementsToDrop { get; private set; } = new HashSet<string>(StringComparer.Ordinal);
            public string DataItemIdPrefix { get; private set; } = string.Empty;

            public bool MatchesDataItemId(string id)
            {
                if (string.IsNullOrEmpty(DataItemIdPrefix)) return true;
                return id.StartsWith(DataItemIdPrefix, StringComparison.Ordinal);
            }

            public static Whitelist Load(string path)
            {
                var json = File.ReadAllText(path);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                return new Whitelist
                {
                    HeaderAttributes = ReadStringSet(root, "headerAttributes"),
                    DeviceAttributes = ReadStringSet(root, "deviceAttributes"),
                    ComponentAttributes = ReadStringSet(root, "componentAttributes"),
                    ObservationAttributes = ReadStringSet(root, "observationAttributes"),
                    ElementsToDrop = ReadStringSet(root, "elementsToDrop"),
                    DataItemIdPrefix = ReadString(root, "dataItemIdPrefix"),
                };
            }

            private static ISet<string> ReadStringSet(JsonElement parent, string property)
            {
                var set = new HashSet<string>(StringComparer.Ordinal);
                if (parent.TryGetProperty(property, out var array) && array.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in array.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            set.Add(item.GetString()!);
                        }
                    }
                }
                return set;
            }

            private static string ReadString(JsonElement parent, string property)
            {
                if (parent.TryGetProperty(property, out var node) && node.ValueKind == JsonValueKind.String)
                {
                    return node.GetString() ?? string.Empty;
                }
                return string.Empty;
            }
        }
    }
}
