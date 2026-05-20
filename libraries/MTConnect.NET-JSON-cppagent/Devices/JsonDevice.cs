// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>Device</c> in the
    /// cppagent-compatible JSON shape. Mirrors the on-the-wire layout, which
    /// wraps child collections in counted container objects, then converts to
    /// and from the strongly-typed <see cref="Device"/> model.
    /// </summary>
    public class JsonDevice
    {
        /// <summary>
        /// The unique <c>id</c> of the device.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

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
        [JsonPropertyName("Description")]
        public JsonDescription Description { get; set; }

        /// <summary>
        /// The configuration (coordinate systems, motion, relationships,
        /// specifications) of the device.
        /// </summary>
        [JsonPropertyName("Configuration")]
        public JsonConfiguration Configuration { get; set; }

        /// <summary>
        /// The data items reported directly by the device, wrapped in a
        /// counted container.
        /// </summary>
        [JsonPropertyName("DataItems")]
        public JsonDataItems DataItems { get; set; }

        /// <summary>
        /// The compositions of the device, wrapped in a counted container.
        /// </summary>
        [JsonPropertyName("Compositions")]
        public JsonCompositions Compositions { get; set; }

        /// <summary>
        /// The child components of the device, wrapped in a counted container.
        /// </summary>
        [JsonPropertyName("Components")]
        public JsonComponents Components { get; set; }

        /// <summary>
        /// The references from the device to other components and data items.
        /// </summary>
        [JsonPropertyName("References")]
        public JsonReferenceContainer References { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDevice() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDevice"/>, wrapping the child collections in their
        /// counted container types.
        /// </summary>
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
        /// unwrapping each child container and parsing the MTConnect version.
        /// </summary>
        public Device ToDevice() => CopyFieldsInto(new Device());

        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Agent"/>.
        /// Agent : Device, so the same set of JsonDevice fields applies; the
        /// difference is the resulting runtime type carries the Agent
        /// discriminator (<c>device is Agent</c>, <c>device.Type == Agent.TypeId</c>).
        /// Callers that read the cppagent JSON v2 envelope must call <c>ToAgent()</c>
        /// for entries deserialised from the "Agent" key inside Devices so the
        /// meta-device's type identity survives the round trip; calling
        /// <see cref="ToDevice"/> in that path silently collapses the discriminator.
        /// </summary>
        public Agent ToAgent() => (Agent)CopyFieldsInto(new Agent());

        private Device CopyFieldsInto(Device device)
        {
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