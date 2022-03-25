// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Agents.Configuration
{
    public class AdapterConfiguration
    {
        /// <summary>
        /// The name of the device that corresponds to the name of the device in the Devices file. Each adapter can map to one device. 
        /// Specifying a "*" will map to the default device.
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// The host the adapter is located on.
        /// </summary>
        [JsonPropertyName("host")]
        public string Host { get; set; }

        /// <summary>
        /// The port to connect to the adapter.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// Replaces the Manufacturer attribute in the device XML.
        /// </summary>
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Replaces the Model attribute in the device XML.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// Replaces the Station attribute in the device XML.
        /// </summary>
        [JsonPropertyName("station")]
        public string Station { get; set; }

        /// <summary>
        /// Replaces the UUID attribute in the device XML.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// For devices that do not have the ability to provide available events, if yes, this sets the Availability to AVAILABLE upon connection.
        /// </summary>
        [JsonPropertyName("autoAvailable")]
        public bool AutoAvailable { get; set; }

        /// <summary>
        /// Comma separated list of additional devices connected to this adapter. This provides availability support when one adapter feeds multiple devices.
        /// </summary>
        [JsonPropertyName("additionalDevices")]
        public List<string> AdditionalDevices { get; set; }

        /// <summary>
        /// If value is true, filters all duplicate values for data items. This is to support adapters that are not doing proper duplicate filtering.
        /// </summary>
        [JsonPropertyName("filterDuplicates")]
        public bool FilterDuplicates { get; set; }

        /// <summary>
        /// The length of time an adapter can be silent before it is disconnected. This is only for legacy adapters that do not support heartbeats. 
        /// If heartbeats are present, this will be ignored.
        /// </summary>
        [JsonPropertyName("legacyTimeout")]
        public int LegacyTimeout { get; set; }

        /// <summary>
        /// The amount of time between adapter reconnection attempts. 
        /// This is useful for implementation of high performance adapters where availability needs to be tracked in near-real-time. 
        /// Time is specified in milliseconds (ms). Defaults to the top level ReconnectInterval.
        /// </summary>
        [JsonPropertyName("reconnectInterval")]
        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("ignoreTimestamps")]
        public bool IgnoreTimestamps { get; set; }

        /// <summary>
        /// Do not overwrite the UUID with the UUID from the adapter, preserve the UUID in the Devices.xml file. This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("preserveUuid")]
        public bool PreserveUuid { get; set; }

        /// <summary>
        /// Boost the thread priority of this adapter so that events are handled faster.
        /// </summary>
        [JsonPropertyName("realTime")]
        public bool RealTime { get; set; }

        /// <summary>
        /// The timestamps will be given as relative offsets represented as a floating point number of milliseconds. 
        /// The offset will be added to the arrival time of the first recorded event.
        /// </summary>
        [JsonPropertyName("relativeTime")]
        public bool RelativeTime { get; set; }

        /// <summary>
        /// Adapter setting for data item units conversion in the agent. Assumes the adapter has already done unit conversion. Defaults to global.
        /// </summary>
        [JsonPropertyName("conversionRequired")]
        public bool ConversionRequired { get; set; }

        /// <summary>
        /// Always converts the value of the data items to upper case.
        /// </summary>
        [JsonPropertyName("upcaseDataItemValue")]
        public bool UpcaseDataItemValue { get; set; }

        /// <summary>
        /// Specifies the SHDR protocol version used by the adapter. When greater than one (1), allows multiple complex observations, like Condition and Message on the same line. 
        /// If it equials one (1), then any observation requiring more than a key/value pair need to be on separate lines. Applies to only this adapter.
        /// </summary>
        [JsonPropertyName("shdrVersion")]
        public string ShdrVersion { get; set; }

        /// <summary>
        /// Suppress the Adapter IP Address and port when creating the Agent Device ids and names for 1.7.
        /// </summary>
        [JsonPropertyName("suppressIpAddress")]
        public bool SuppressIpAddress { get; set; }


        public AdapterConfiguration()
        {
            Device = "*";
            Host = "localhost";
            Port = 7878;

            AutoAvailable = false;
            AdditionalDevices = null;
            FilterDuplicates = false;
            LegacyTimeout = 600;
            ReconnectInterval = 10000;
            IgnoreTimestamps = false;
            PreserveUuid = false;
            RealTime = false;
            RelativeTime = false;
            ConversionRequired = true;
            UpcaseDataItemValue = true;
            ShdrVersion = "1";
            SuppressIpAddress = false;
        }
    }
}
