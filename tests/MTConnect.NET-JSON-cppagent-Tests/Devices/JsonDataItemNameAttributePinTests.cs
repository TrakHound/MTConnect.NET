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
    /// Metadata-level pin for the JSON-cppagent Probe DataItem `name` contract.
    ///
    /// The constructor-side guard (`if (!string.IsNullOrEmpty(name)) Name = name`) only
    /// covers the runtime path through MTConnect.NET's own JSON helper which already
    /// ignores defaulted strings via `JsonFunctions.DefaultOptions`. Down-stream
    /// consumers (dime-connector, third-party code) that probe the property metadata
    /// directly — or hand the DTO to a fresh `JsonSerializer` instance with no project
    /// options — must still see the property omitted when null. The
    /// `[JsonIgnore(Condition = WhenWritingNull)]` attribute pins that contract on the
    /// type itself so it travels with the DTO.
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd —
    ///   `DataItem/@name` is declared `use="optional"`. An optional attribute with no
    ///   value must be omitted from the wire.
    /// - cppagent reference: lib/mtconnect/printer/json_printer_helper.hpp — the printer
    ///   skips fields whose underlying optional has no value, never emits empty.
    /// - Public defect tracker: https://github.com/TrakHound/MTConnect.NET/issues/138.
    /// - CONVENTIONS §15: tests on serialization contracts cite spec + reference impl.
    /// </summary>
    [TestFixture]
    [Category("NameAttributeOmissionWhenUnsetOrEmpty")]
    public class JsonDataItemNameAttributePinTests
    {
        [Test]
        public void Name_property_carries_JsonIgnore_WhenWritingNull_attribute()
        {
            var prop = typeof(JsonDataItem).GetProperty(nameof(JsonDataItem.Name));
            var attr = prop?.GetCustomAttribute<JsonIgnoreAttribute>();
            Assert.That(attr, Is.Not.Null,
                "JsonDataItem.Name must carry [JsonIgnore] so raw System.Text.Json serialisation omits the property when null.");
            Assert.That(attr!.Condition, Is.EqualTo(JsonIgnoreCondition.WhenWritingNull),
                "JsonDataItem.Name [JsonIgnore] condition must be WhenWritingNull.");
        }

        [Test]
        public void Raw_System_Text_Json_serialise_omits_name_when_null()
        {
            var dto = new JsonDataItem { Id = "x", Type = "T", Name = null };
            var json = JsonSerializer.Serialize(dto);
            using var doc = JsonDocument.Parse(json);
            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "Raw System.Text.Json (no JsonFunctions.DefaultOptions) must still omit 'name' when null on JsonDataItem.");
        }
    }
}
