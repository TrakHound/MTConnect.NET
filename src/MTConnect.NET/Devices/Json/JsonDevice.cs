﻿// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// The primary container element of each device. 
    /// Device is contained within the top level Devices container. 
    /// There MAY be multiple Device elements in an XML document.
    /// </summary>
    public class JsonDevice
    {
        /// <summary>
        /// The unique identifier for this Device in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the Device.
        /// THis name should be unique within the XML document to allow for easier data integration.
        /// An NMTOKEN XML type.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier that will only refer ot this Device.
        /// For example, this may be the manufacturer's code and the serial number.
        /// The uuid shoudl be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        [JsonPropertyName("iso841Class")]
        public string Iso841Class { get; set; }

        /// <summary>
        /// The name the device manufacturer assigned to this Device.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a device until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        [JsonPropertyName("sampleInterval")]
        public double? SampleInterval { get; set; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The MTConnect version of the Devices Information Model used to configure
        /// the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        [JsonPropertyName("mtconnectVersion")]
        public string MTConnectVersion { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        [JsonPropertyName("description")]
        public Description Description { get; set; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        [JsonPropertyName("configuration")]
        public Configuration Configuration { get; set; }

        /// <summary>
        /// A container for the Data Entities associated with this Component element.
        /// </summary>
        [JsonPropertyName("dataItems")]
        public List<JsonDataItem> DataItems { get; set; }

        /// <summary>
        /// A container for Lower Level Component XML elements associated with this parent Component.
        /// </summary>
        [JsonPropertyName("components")]
        public List<JsonComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        [JsonPropertyName("compositions")]
        public List<JsonComposition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        [JsonPropertyName("references")]
        public List<Reference> References { get; set; }


        public JsonDevice() { }

        public JsonDevice(Device device)
        {
            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                Description = device.Description;
                if (device.SampleRate > 0) SampleRate = device.SampleRate;
                if (device.SampleInterval > 0) SampleInterval = device.SampleInterval;
                Iso841Class = device.Iso841Class;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                if (device.MTConnectVersion != null) MTConnectVersion = device.MTConnectVersion.ToString();
                Configuration = device.Configuration;
                References = device.References;
                Description = device.Description;


                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    DataItems = new List<JsonDataItem>();

                    foreach (var dataItem in device.DataItems)
                    {
                        DataItems.Add(new JsonDataItem(dataItem));
                    }
                }

                // Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    Compositions = new List<JsonComposition>();

                    foreach (var composition in device.Compositions)
                    {
                        Compositions.Add(new JsonComposition(composition));
                    }
                }

                // Components
                if (!device.Components.IsNullOrEmpty())
                {
                    Components = new List<JsonComponent>();

                    foreach (var component in device.Components)
                    {
                        Components.Add(new JsonComponent(component));
                    }
                }
            }
        }

        public Device ToDevice()
        {
            var device = new Device();

            device.Id = Id;
            device.Name = Name;
            device.NativeName = NativeName;
            device.Uuid = Uuid;
            device.Description = Description;
            device.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            device.SampleInterval = SampleInterval.HasValue ? SampleInterval.Value : 0;
            device.Iso841Class = Iso841Class;
            device.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Version.TryParse(MTConnectVersion, out var mtconnectVersion))
            {
                device.MTConnectVersion = mtconnectVersion;
            }
            device.Configuration = Configuration;
            device.References = References;
            device.Description = Description;

            // DataItems
            if (!DataItems.IsNullOrEmpty())
            {
                device.DataItems = new List<DataItem>();
                foreach (var dataItem in DataItems)
                {
                    device.DataItems.Add(dataItem.ToDataItem());
                }
            }

            // Compositions
            if (!Compositions.IsNullOrEmpty())
            {
                device.Compositions = new List<Composition>();
                foreach (var composition in Compositions)
                {
                    device.Compositions.Add(composition.ToComposition());
                }
            }

            // Components
            if (!Components.IsNullOrEmpty())
            {
                device.Components = new List<Component>();
                foreach (var component in Components)
                {
                    device.Components.Add(component.ToComponent());
                }
            }

            return device;
        }
    }
}
