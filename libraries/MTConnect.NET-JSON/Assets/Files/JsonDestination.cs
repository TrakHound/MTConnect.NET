// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for a file <c>Destination</c>, the device
    /// the file is intended for. Converts to and from the strongly-typed
    /// <see cref="Destination"/> model.
    /// </summary>
    public class JsonDestination
    {
        /// <summary>
        /// The UUID of the destination device.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
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
        /// Converts this surrogate to a strongly-typed <see cref="IDestination"/>.
        /// </summary>
        public IDestination ToDestination()
        {
            var destination = new Destination();
            destination.DeviceUuid = DeviceUuid;
            return destination;
        }
    }
}