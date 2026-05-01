// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MTConnect.Devices.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// Metadata-level pin for the JSON-cppagent Probe Component `name` contract.
    /// Mirrors JsonDataItemNameAttributePinTests — the constructor guard only covers
    /// the project's own helper path; metadata-level omission is required so external
    /// consumers (dime-connector et al.) that hit the type with a fresh JsonSerializer
    /// still see `name` skipped on null.
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd —
    ///   Component `@name` is `use="optional"`.
    /// - cppagent reference: lib/mtconnect/printer/json_printer_helper.hpp omits
    ///   absent optionals.
    /// - Public defect tracker: https://github.com/TrakHound/MTConnect.NET/issues/138.
    /// - CONVENTIONS §15: tests on serialization contracts cite spec + reference impl.
    /// </summary>
    [TestFixture]
    [Category("NameAttributeOmissionWhenUnsetOrEmpty")]
    public class JsonComponentNameAttributePinTests
    {
        [Test]
        public void Name_property_carries_JsonIgnore_WhenWritingNull_attribute()
        {
            var prop = typeof(JsonComponent).GetProperty(nameof(JsonComponent.Name));
            var attr = prop?.GetCustomAttribute<JsonIgnoreAttribute>();
            Assert.That(attr, Is.Not.Null,
                "JsonComponent.Name must carry [JsonIgnore] so raw System.Text.Json serialisation omits the property when null.");
            Assert.That(attr!.Condition, Is.EqualTo(JsonIgnoreCondition.WhenWritingNull),
                "JsonComponent.Name [JsonIgnore] condition must be WhenWritingNull.");
        }

        [Test]
        public void Raw_System_Text_Json_serialise_omits_name_when_null()
        {
            var dto = new JsonComponent { Id = "x", Name = null };
            var json = JsonSerializer.Serialize(dto);
            using var doc = JsonDocument.Parse(json);
            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "Raw System.Text.Json (no JsonFunctions.DefaultOptions) must still omit 'name' when null on JsonComponent.");
        }
    }
}
