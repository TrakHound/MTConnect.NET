// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>Device</c>, the
    /// top-level component of an MTConnectDevices document. Mirrors the
    /// on-the-wire shape so the JSON serializer can read and write it, then
    /// converts to and from the strongly-typed <see cref="Device"/> model.
    /// </summary>
    public class JsonDevice
    {
        /// <summary>
        /// The unique <c>id</c> of the device.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The MTConnect device <c>type</c> (typically Device or Agent).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The human-readable <c>name</c> of the device.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The universally unique identifier of the device.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// The hash uniquely identifying the device's current configuration.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// The ISO 841 classification of the device.
        /// </summary>
        [JsonPropertyName("iso841Class")]
        public string Iso841Class { get; set; }

        /// <summary>
        /// The name the device is known by in the native data source, when it
        /// differs from <see cref="Name"/>.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval, in milliseconds, between samples of the device's
        /// data, when reported.
        /// </summary>
        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        /// <summary>
        /// The rate, in samples per second, at which the device's data is
        /// sampled, when reported.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of a CoordinateSystem the device's
        /// values are expressed relative to.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The MTConnect schema version the device definition conforms to.
        /// </summary>
        [JsonPropertyName("mtconnectVersion")]
        public string MTConnectVersion { get; set; }

        /// <summary>
        /// The descriptive metadata (manufacturer, model, serial number) for
        /// the device.
        /// </summary>
        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        /// <summary>
        /// The configuration (coordinate systems, motion, relationships,
        /// specifications) of the device.
        /// </summary>
        [JsonPropertyName("configuration")]
        public JsonConfiguration Configuration { get; set; }

        /// <summary>
        /// The data items reported directly by the device.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public IEnumerable<JsonDataItem> DataItems { get; set; }

        /// <summary>
        /// The child components of the device.
        /// </summary>
        [JsonPropertyName("components")]
        public IEnumerable<JsonComponent> Components { get; set; }

        /// <summary>
        /// The compositions (lower-level structural elements) of the device.
        /// </summary>
        [JsonPropertyName("compositions")]
        public IEnumerable<JsonComposition> Compositions { get; set; }

        /// <summary>
        /// The references from the device to other components and data items.
        /// </summary>
        [JsonPropertyName("references")]
        public JsonReferenceContainer References { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDevice() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDevice"/>, recursively converting the description,
        /// configuration, references, data items, compositions, and
        /// components.
        /// </summary>
        public JsonDevice(IDevice device)
        {
            if (device != null)
            {
                Id = device.Id;
                Type = device.Type;
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


        /// <summary>
        /// Returns the JSON representation of this surrogate, optionally
        /// indented for readability.
        /// </summary>
        public string ToString(bool indent = false) => JsonFunctions.Convert(this, indented: indent);

        /// <summary>
        /// Returns the UTF-8 encoded JSON bytes of this surrogate, optionally
        /// indented for readability.
        /// </summary>
        public byte[] ToBytes(bool indent = false) => JsonFunctions.ConvertBytes(this, indented: indent);

        /// <summary>
        /// Returns a stream over the JSON representation of this surrogate,
        /// optionally indented for readability.
        /// </summary>
        public Stream ToStream(bool indent = false) => JsonFunctions.ConvertStream(this, indented: indent);


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Device"/>,
        /// parsing the MTConnect version and recursively converting the
        /// description, configuration, references, data items, compositions,
        /// and components.
        /// </summary>
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