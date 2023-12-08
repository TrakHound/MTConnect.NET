// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.References;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

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

        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("configuration")]
        public JsonConfiguration Configuration { get; set; }

        [JsonPropertyName("dataItems")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }

        [JsonPropertyName("components")]
        public IEnumerable<JsonComponent> Components { get; set; }

        [JsonPropertyName("compositions")]
        public IEnumerable<JsonComposition> Compositions { get; set; }

        [JsonPropertyName("references")]
        public JsonReferenceContainer References { get; set; }


        public JsonDevice() { }

        public JsonDevice(IDevice device)
        {
            if (device != null)
            {
                Id = device.Id;
                Type = device.Type;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                InstanceId = device.InstanceId;
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
                    var dataItems = new List<JsonDataItem>();
                    foreach (var dataItem in device.DataItems)
                    {
                        dataItems.Add(new JsonDataItem(dataItem));
                    }
                    DataItems = dataItems;
                }

                // Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    var compositions = new List<JsonComposition>();
                    foreach (var composition in device.Compositions)
                    {
                        compositions.Add(new JsonComposition(composition));
                    }
                    Compositions = compositions;
                }

                // Components
                if (!device.Components.IsNullOrEmpty())
                {
                    var components = new List<JsonComponent>();
                    foreach (var component in device.Components)
                    {
                        components.Add(new JsonComponent(component));
                    }
                    Components = components;
                }
            }
        }


        public string ToString(bool indent = false) => JsonFunctions.Convert(this, indented: indent);

        public byte[] ToBytes(bool indent = false) => JsonFunctions.ConvertBytes(this, indented: indent);


        public Device ToDevice()
        {
            var device = new Device();

            device.Id = Id;
            device.Name = Name;
            device.NativeName = NativeName;
            device.Uuid = Uuid;
            device.InstanceId = InstanceId;
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
            if (!DataItems.IsNullOrEmpty())
            {
                var dataItems = new List<DataItem>();
                foreach (var dataItem in DataItems)
                {
                    dataItems.Add(dataItem.ToDataItem());
                }
                device.DataItems = dataItems;
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                var compositions = new List<Composition>();
                foreach (var composition in Compositions)
                {
                    compositions.Add(composition.ToComposition());
                }
                device.Compositions = compositions;  
            }

            // Components
            if (!Components.IsNullOrEmpty())
            {
                var components = new List<Component>();
                foreach (var component in Components)
                {
                    components.Add(component.ToComponent());
                }
                device.Components = components;
            }

            return device;
        }
    }
}