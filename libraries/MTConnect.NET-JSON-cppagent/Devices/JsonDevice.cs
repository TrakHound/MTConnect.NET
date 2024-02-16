// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("iso841Class")]
        public string Iso841Class { get; set; }

        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [JsonPropertyName("mtconnectVersion")]
        public string MTConnectVersion { get; set; }

        [JsonPropertyName("Description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("Configuration")]
        public JsonConfiguration Configuration { get; set; }

        [JsonPropertyName("DataItems")]
        public JsonDataItems DataItems { get; set; }

        [JsonPropertyName("Compositions")]
        public JsonCompositions Compositions { get; set; }

        [JsonPropertyName("Components")]
        public JsonComponents Components { get; set; }

        [JsonPropertyName("References")]
        public JsonReferenceContainer References { get; set; }


        public JsonDevice() { }

        public JsonDevice(IDevice device)
        {
            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                Hash = device.Hash;
                if (device.SampleRate > 0) SampleRate = device.SampleRate;
                if (device.SampleInterval > 0) SampleInterval = device.SampleInterval;
                Iso841Class = device.Iso841Class;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                if (device.MTConnectVersion != null) MTConnectVersion = device.MTConnectVersion.ToString();
                if (device.Description != null) Description = new JsonDescription(device.Description);

                // References
                if (!device.References.IsNullOrEmpty()) References = new JsonReferenceContainer(device.References);

                // Configuration
                if (device.Configuration != null) Configuration = new JsonConfiguration(device.Configuration);

                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    DataItems = new JsonDataItems(device.DataItems);
                }

                // Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    Compositions = new JsonCompositions(device.Compositions);
                }

                // Components
                if (!device.Components.IsNullOrEmpty())
                {
                    Components = new JsonComponents(device.Components);
                }
            }
        }


        public string ToString(bool indent = false) => JsonFunctions.Convert(this, indented: indent);
        public byte[] ToBytes(bool indent = false) => JsonFunctions.ConvertBytes(this, indented: indent);
        public Stream ToStream(bool indent = false) => JsonFunctions.ConvertStream(this, indented: indent);

        public Device ToDevice()
        {
            var device = new Device();

            device.Id = Id;
            device.Name = Name;
            device.NativeName = NativeName;
            device.Uuid = Uuid;
            device.Hash = Hash;
            device.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            device.SampleInterval = SampleInterval.HasValue ? SampleInterval.Value : 0;
            device.Iso841Class = Iso841Class;
            device.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Version.TryParse(MTConnectVersion, out var mtconnectVersion))
            {
                device.MTConnectVersion = mtconnectVersion;
            }

            if (Description != null) device.Description = Description.ToDescription();

            // References
            if (References != null) device.References = References.ToReferences();

            // Configuration
            if (Configuration != null) device.Configuration = Configuration.ToConfiguration();

            // DataItems
            if (DataItems != null) device.DataItems = DataItems.ToDataItems();

            // Compositions
            if (Compositions != null) device.Compositions = Compositions.ToCompositions();

            // Components
            if (Components != null) device.Components = Components.ToComponents();

            return device;
        }
    }
}