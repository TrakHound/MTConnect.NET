// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// Pins the JSON-cppagent Probe Component behavior: the `name` JSON property must be
    /// omitted when the source IComponent.Name is null or empty, and emitted with the
    /// original value otherwise. Same rationale as the sibling DataItem fixture — every
    /// optional NameType slot in the v2.7 Devices XSD is `use="optional"` and the
    /// reference cppagent printer omits the attribute when the model-side value is null.
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd —
    ///   Component `name` is declared `use="optional"`.
    /// - Reference shape: libraries/MTConnect.NET-XML/Devices/XmlComponent.cs already
    ///   guards the XML attribute write with `string.IsNullOrEmpty(component.Name)`.
    /// - Public defect tracker: https://github.com/TrakHound/MTConnect.NET/issues/138.
    /// </summary>
    [TestFixture]
    [Category("NameAttributeOmissionWhenUnsetOrEmpty")]
    public class JsonComponentEmptyNameOmissionTests
    {
        [Test]
        public void Constructor_with_null_Name_source_does_not_serialize_name_key()
        {
            var source = new Component { Id = "axis-1", Name = null };
            var json = new JsonComponent(source).ToString();
            using var doc = JsonDocument.Parse(json);
            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "JSON-cppagent Probe Component must omit 'name' when source Name is null");
        }

        [Test]
        public void Constructor_with_empty_Name_source_does_not_serialize_name_key()
        {
            var source = new Component { Id = "axis-2", Name = string.Empty };
            var json = new JsonComponent(source).ToString();
            using var doc = JsonDocument.Parse(json);
            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "JSON-cppagent Probe Component must omit 'name' when source Name is empty");
        }

        [Test]
        public void Constructor_with_explicit_Name_source_serializes_name_key()
        {
            var source = new Component { Id = "axis-3", Name = "X" };
            var json = new JsonComponent(source).ToString();
            using var doc = JsonDocument.Parse(json);
            Assert.That(doc.RootElement.TryGetProperty("name", out var nameElement), Is.True,
                "JSON-cppagent Probe Component must emit 'name' when source Name has a value");
            Assert.That(nameElement.GetString(), Is.EqualTo("X"));
        }
    }
}
