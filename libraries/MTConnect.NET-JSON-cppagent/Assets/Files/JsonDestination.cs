// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Destination</c> on a File
    /// asset, identifying a downstream device by UUID that the file
    /// should be delivered to. The cppagent shape emits the device UUID
    /// under the <c>value</c> key (rather than <c>deviceUuid</c>) — see
    /// the inline note on <see cref="DeviceUuid"/>.
    /// </summary>
    public class JsonDestination
    {
        /// <summary>
        /// UUID of the destination device. Note the JSON key is
        /// <c>value</c> in the cppagent shape even though the XML
        /// attribute is <c>deviceUuid</c>.
        /// </summary>
        [JsonPropertyName("value")] // Not sure why this is "value" when it is listed as an XML attribute with the name of "deviceUuid"
        public string DeviceUuid { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDestination() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDestination"/>.
        /// </summary>
        public JsonDestination(IDestination destination)
        {
            if (destination != null)
            {
                DeviceUuid = destination.DeviceUuid;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IDestination"/>.
        /// </summary>
        public IDestination ToDestination()
        {
            var destination = new Destination();
            destination.DeviceUuid = DeviceUuid;
            return destination;
        }
    }
}