// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using MTConnect.Devices.Json;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Regressions
{
    /// <summary>
    /// Pins the envelope-vs-Header schemaVersion contract for the
    /// cppagent JSON wire format. Both fields must exist independently
    /// and continue to be wired through their own
    /// <see cref="JsonPropertyNameAttribute"/>:
    ///
    ///   - <c>JsonMTConnectStreams.SchemaVersion</c>     -> JSON key <c>"schemaVersion"</c> at envelope root
    ///   - <c>JsonMTConnectDevices.SchemaVersion</c>     -> JSON key <c>"schemaVersion"</c> at envelope root
    ///   - <c>JsonDevicesHeader.SchemaVersion</c>        -> JSON key <c>"schemaVersion"</c> nested inside Header
    ///
    /// The two SchemaVersion fields are populated from independent
    /// sources and are NOT interchangeable — the envelope field
    /// identifies the document schema the producer emitted while the
    /// Header field identifies the agent's configured MTConnect
    /// Standard release. Removing either field, or repointing one to
    /// the other's source, would silently change the wire shape and
    /// corrupt downstream consumers.
    ///
    /// The fixture also pins that each field carries an XML doc
    /// comment that explains the envelope-vs-Header semantics so
    /// future maintainers cannot accidentally collapse the two.
    /// XML doc presence is enforced by reading the source file
    /// (parsed XML doc files are not deployed alongside the assembly
    /// in this repo).
    ///
    /// Sources:
    ///   - cppagent JSON envelope: https://github.com/mtconnect/cppagent
    ///     v2.7.0.7 reference printer emits both fields independently.
    ///   - Issue: https://github.com/TrakHound/MTConnect.NET/issues/128
    /// </summary>
    [TestFixture]
    [Category("WireShape")]
    public class SchemaVersionFieldsCoexistTests
    {
        [TestCase(typeof(JsonMTConnectStreams), "envelope")]
        [TestCase(typeof(JsonMTConnectDevices), "envelope")]
        [TestCase(typeof(JsonDevicesHeader),    "Header")]
        public void SchemaVersion_property_exists_with_camelCase_json_key(
            System.Type carrier, string surface)
        {
            var prop = carrier.GetProperty(
                "SchemaVersion", BindingFlags.Public | BindingFlags.Instance);

            Assert.That(prop, Is.Not.Null,
                $"`{carrier.Name}` ({surface}) must expose a `SchemaVersion` property; " +
                "removing it silently regresses the cppagent JSON wire shape.");

            var attribute = prop!.GetCustomAttribute<JsonPropertyNameAttribute>();
            Assert.That(attribute, Is.Not.Null,
                $"`{carrier.Name}.SchemaVersion` must carry a [JsonPropertyName] attribute; " +
                "the JSON key cannot be inferred from the property name alone.");
            Assert.That(attribute!.Name, Is.EqualTo("schemaVersion"),
                $"`{carrier.Name}.SchemaVersion` must serialize as the camelCase JSON key " +
                $"`schemaVersion` (the cppagent wire-shape convention for scalar attributes).");
        }

        [Test]
        public void Streams_envelope_and_devices_envelope_carry_independent_SchemaVersion_fields()
        {
            // Both envelopes have their own SchemaVersion. They are NOT
            // shared via inheritance or composition, so a future refactor
            // that consolidates them would also need to update this pin.
            var streamsProp = typeof(JsonMTConnectStreams).GetProperty(
                "SchemaVersion", BindingFlags.Public | BindingFlags.Instance);
            var devicesProp = typeof(JsonMTConnectDevices).GetProperty(
                "SchemaVersion", BindingFlags.Public | BindingFlags.Instance);

            Assert.That(streamsProp, Is.Not.Null);
            Assert.That(devicesProp, Is.Not.Null);
            Assert.That(streamsProp!.DeclaringType, Is.EqualTo(typeof(JsonMTConnectStreams)),
                "JsonMTConnectStreams.SchemaVersion must be declared on the Streams envelope itself, " +
                "not inherited from a shared base — the field is wired from streamsDocument.Version.");
            Assert.That(devicesProp!.DeclaringType, Is.EqualTo(typeof(JsonMTConnectDevices)),
                "JsonMTConnectDevices.SchemaVersion must be declared on the Devices envelope itself, " +
                "not inherited from a shared base — the field is wired from document.Version.");
        }

        [Test]
        public void Devices_envelope_SchemaVersion_distinct_from_Header_SchemaVersion()
        {
            // The Devices envelope has its own SchemaVersion AND nests a
            // Header which also has its own SchemaVersion. Both must
            // coexist — collapsing them would conflate "what wire format
            // did the producer emit" with "what Standard release does the
            // agent run".
            var envelopeProp = typeof(JsonMTConnectDevices).GetProperty(
                "SchemaVersion", BindingFlags.Public | BindingFlags.Instance);
            var headerProp = typeof(JsonDevicesHeader).GetProperty(
                "SchemaVersion", BindingFlags.Public | BindingFlags.Instance);

            Assert.That(envelopeProp, Is.Not.Null);
            Assert.That(headerProp, Is.Not.Null);
            Assert.That(envelopeProp!.DeclaringType, Is.Not.EqualTo(headerProp!.DeclaringType),
                "Envelope and Header SchemaVersion fields must live on distinct types so " +
                "they can be populated from independent sources.");
        }

        // The XML doc presence guard. Reads the committed source files
        // (XML doc XML output is not deployed) so a future maintainer
        // cannot delete the doc comments without tripping a guard.
        [TestCase(
            "libraries/MTConnect.NET-JSON-cppagent/Streams/JsonMTConnectStreams.cs",
            "envelope")]
        [TestCase(
            "libraries/MTConnect.NET-JSON-cppagent/Devices/JsonMTConnectDevices.cs",
            "envelope")]
        [TestCase(
            "libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevicesHeader.cs",
            "Header")]
        public void SchemaVersion_property_carries_envelope_vs_Header_xml_doc(
            string relativeSourcePath, string surface)
        {
            var path = Path.Combine(RepoRootLocator.LocateRoot(), relativeSourcePath);
            Assert.That(File.Exists(path), Is.True, $"Expected source file at '{path}'.");

            var text = File.ReadAllText(path);

            // Locate the SchemaVersion property and walk back to the doc
            // comment that immediately precedes it. The doc must contain
            // the words "envelope" AND "Header" so the contrast between
            // the two surfaces stays explicit.
            var anchor = text.IndexOf("public string SchemaVersion", System.StringComparison.Ordinal);
            Assert.That(anchor, Is.GreaterThan(0),
                $"`{relativeSourcePath}` must declare `public string SchemaVersion`.");

            // Look at the 600 chars preceding the property declaration —
            // doc comments are bounded by `/// </summary>` tags.
            var windowStart = System.Math.Max(0, anchor - 800);
            var window = text.Substring(windowStart, anchor - windowStart);

            Assert.That(window, Does.Contain("///"),
                $"`{relativeSourcePath}` ({surface}): the `SchemaVersion` property must " +
                "carry an XML doc comment explaining the envelope-vs-Header semantics.");
            Assert.That(window.ToLowerInvariant(), Does.Contain("envelope"),
                $"`{relativeSourcePath}` ({surface}): SchemaVersion XML doc must mention " +
                "the word \"envelope\" so the contrast with the Header field is explicit.");
            Assert.That(window, Does.Contain("Header"),
                $"`{relativeSourcePath}` ({surface}): SchemaVersion XML doc must mention " +
                "the word \"Header\" so the contrast with the envelope field is explicit.");
        }
    }
}
