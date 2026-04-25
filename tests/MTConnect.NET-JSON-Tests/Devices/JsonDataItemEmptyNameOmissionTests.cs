// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.Json;
using NUnit.Framework;

namespace MTConnect.Tests.Json.Devices
{
    /// <summary>
    /// Pins the base JSON Probe DataItem behavior: the `name` JSON property must be
    /// omitted when the source IDataItem.Name is null or empty, and emitted with the
    /// original value otherwise.
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd —
    ///   `DataItem/@name` is declared `use="optional"`. An optional attribute with no
    ///   value must be omitted from the wire, not emitted as an empty placeholder.
    /// - Reference shape: libraries/MTConnect.NET-XML/Devices/XmlDataItem.cs already
    ///   guards the XML attribute write with `string.IsNullOrEmpty(dataItem.Name)`.
    /// - Public defect tracker: https://github.com/TrakHound/MTConnect.NET/issues/138.
    /// </summary>
    [TestFixture]
    [Category("NameAttributeOmissionWhenUnsetOrEmpty")]
    public class JsonDataItemEmptyNameOmissionTests
    {
        [Test]
        public void Constructor_with_null_Name_source_does_not_serialize_name_key()
        {
            var source = new DataItem
            {
                Id = "item-1",
                Type = "TEMPERATURE",
                Category = DataItemCategory.SAMPLE,
                Name = null
            };

            var json = new JsonDataItem(source).ToString();
            using var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "Base JSON Probe DataItem must omit 'name' when source Name is null");
        }

        [Test]
        public void Constructor_with_empty_Name_source_does_not_serialize_name_key()
        {
            var source = new DataItem
            {
                Id = "item-2",
                Type = "TEMPERATURE",
                Category = DataItemCategory.SAMPLE,
                Name = string.Empty
            };

            var json = new JsonDataItem(source).ToString();
            using var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "Base JSON Probe DataItem must omit 'name' when source Name is empty");
        }

        [Test]
        public void Constructor_with_explicit_Name_source_serializes_name_key()
        {
            var source = new DataItem
            {
                Id = "item-3",
                Type = "TEMPERATURE",
                Category = DataItemCategory.SAMPLE,
                Name = "temp"
            };

            var json = new JsonDataItem(source).ToString();
            using var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.TryGetProperty("name", out var nameElement), Is.True,
                "Base JSON Probe DataItem must emit 'name' when source Name has a value");
            Assert.That(nameElement.GetString(), Is.EqualTo("temp"));
        }

        [Test]
        public void Constructor_with_typed_DataItem_unset_Name_does_not_serialize_name_key()
        {
            var source = new TemperatureDataItem
            {
                Id = "item-4",
                Name = string.Empty
            };

            var json = new JsonDataItem(source).ToString();
            using var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.TryGetProperty("name", out _), Is.False,
                "Cleared Name on a typed DataItem must produce no 'name' key");
        }
    }
}
