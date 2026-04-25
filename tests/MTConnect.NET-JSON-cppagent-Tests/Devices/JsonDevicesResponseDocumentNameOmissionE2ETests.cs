// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.Json;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// End-to-end serialization pin: constructs a programmatic Device with a mix
    /// of named and unnamed DataItems, runs it through the full
    /// `JsonDevicesResponseDocument` serializer (the cppagent Probe response wire
    /// path), and asserts the JSON output omits the `name` key on the unnamed
    /// items and keeps it on the named ones. This exercises the surface a real
    /// MQTT/HTTP agent goes through, with the runtime serializer options applied.
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd —
    ///   `DataItem/@name` is `use="optional"`. The Probe response document MUST
    ///   omit the attribute when no value is supplied.
    /// - Public defect tracker: https://github.com/TrakHound/MTConnect.NET/issues/138.
    /// </summary>
    [TestFixture]
    public class JsonDevicesResponseDocumentNameOmissionE2ETests
    {
        [Test]
        public void Probe_response_omits_name_on_unnamed_dataitems_keeps_it_on_named()
        {
            // Build a Device with one Heating component containing two
            // Temperature DataItems: one cleared name, one explicit name.
            var heating = new HeatingComponent { Name = "MainController" };
            heating.AddDataItem(new TemperatureDataItem("dev")
            {
                Id = "t1",
                Name = string.Empty,
                Units = "CELSIUS"
            });
            heating.AddDataItem(new TemperatureDataItem("dev")
            {
                Id = "t2",
                Name = "temp",
                Units = "CELSIUS"
            });

            var device = new Device
            {
                Id = "dev",
                Name = "ExampleDevice",
                Uuid = "ExampleDevice"
            };
            device.AddComponent(heating);

            var document = new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader { Version = "6.9.0" },
                Devices = new List<IDevice> { device }
            };

            var json = JsonFunctions.Convert(new JsonDevicesResponseDocument(document));
            Assert.That(json, Is.Not.Null);
            using var doc = JsonDocument.Parse(json!);

            // Drill into the Heating component's DataItems. The cppagent JSON
            // shape wraps Device and Component arrays in named container
            // objects: `Devices.Device[]`, `Components.Heating[]`,
            // `DataItems.DataItem[]`.
            var heatingDataItems = doc.RootElement
                .GetProperty("MTConnectDevices")
                .GetProperty("Devices")
                .GetProperty("Device")[0]
                .GetProperty("Components")
                .GetProperty("Heating")[0]
                .GetProperty("DataItems")
                .GetProperty("DataItem");

            // The component contains exactly two DataItems; one is the cleared
            // name (no `name` key on the wire), the other is the explicit
            // `name = "temp"`.
            Assert.That(heatingDataItems.GetArrayLength(), Is.EqualTo(2));

            // Identify the items by the presence/value of the `name` key.
            JsonElement clearedItem = default;
            JsonElement explicitItem = default;
            foreach (var item in heatingDataItems.EnumerateArray())
            {
                if (item.TryGetProperty("name", out var nameElement))
                    explicitItem = item;
                else
                    clearedItem = item;
            }

            Assert.That(clearedItem.ValueKind, Is.EqualTo(JsonValueKind.Object),
                "Expected one DataItem without a 'name' key");
            Assert.That(clearedItem.GetProperty("type").GetString(), Is.EqualTo("TEMPERATURE"));

            Assert.That(explicitItem.ValueKind, Is.EqualTo(JsonValueKind.Object),
                "Expected one DataItem with a 'name' key");
            Assert.That(explicitItem.GetProperty("name").GetString(), Is.EqualTo("temp"));

            // Wire-shape regression check: no DataItem object in the document
            // exposes the bug's `"name": ""` placeholder.
            Assert.That(json, Does.Not.Contain("\"name\":\"\""));
        }

    }
}
