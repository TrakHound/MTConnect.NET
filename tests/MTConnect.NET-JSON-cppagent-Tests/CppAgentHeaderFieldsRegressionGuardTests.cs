// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;
using System.Text.Json.Serialization;
using MTConnect.Assets.Json;
using MTConnect.Devices.Json;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent
{
    /// <summary>
    /// Reflection guard. Pins the contract: every JSON-cppagent header DTO must
    /// expose `schemaVersion` and `testIndicator` as serializable properties so a
    /// future regression cannot silently drop either field from the wire.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every Streams / Devices / Assets envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130 (schemaVersion),
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131 (testIndicator).
    /// </summary>
    [TestFixture]
    public class CppAgentHeaderFieldsRegressionGuardTests
    {
        private static readonly System.Type[] HeaderDtos =
        {
            typeof(JsonStreamsHeader),
            typeof(JsonDevicesHeader),
            typeof(JsonAssetsHeader),
        };

        [TestCaseSource(nameof(HeaderDtos))]
        public void Header_dto_exposes_schemaVersion_property(System.Type headerType)
        {
            var property = headerType.GetProperty("SchemaVersion",
                BindingFlags.Public | BindingFlags.Instance);

            Assert.That(property, Is.Not.Null,
                $"{headerType.Name} must expose a public SchemaVersion property.");
            Assert.That(property!.PropertyType, Is.EqualTo(typeof(string)),
                $"{headerType.Name}.SchemaVersion must be a string.");

            var jsonAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            Assert.That(jsonAttr, Is.Not.Null,
                $"{headerType.Name}.SchemaVersion must carry [JsonPropertyName] for cppagent wire shape.");
            Assert.That(jsonAttr!.Name, Is.EqualTo("schemaVersion"),
                $"{headerType.Name}.SchemaVersion must serialize as 'schemaVersion'.");
        }

        [TestCaseSource(nameof(HeaderDtos))]
        public void Header_dto_exposes_testIndicator_property(System.Type headerType)
        {
            var property = headerType.GetProperty("TestIndicator",
                BindingFlags.Public | BindingFlags.Instance);

            Assert.That(property, Is.Not.Null,
                $"{headerType.Name} must expose a public TestIndicator property.");
            Assert.That(property!.PropertyType, Is.EqualTo(typeof(bool)),
                $"{headerType.Name}.TestIndicator must be a bool.");

            var jsonAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            Assert.That(jsonAttr, Is.Not.Null,
                $"{headerType.Name}.TestIndicator must carry [JsonPropertyName] for cppagent wire shape.");
            Assert.That(jsonAttr!.Name, Is.EqualTo("testIndicator"),
                $"{headerType.Name}.TestIndicator must serialize as 'testIndicator'.");
        }
    }
}
