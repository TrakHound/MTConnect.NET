// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Description</c> in the
    /// cppagent-compatible shape. Carries the descriptive metadata
    /// (manufacturer, model, serial number, station) plus the free-form
    /// description text. Converts to and from the strongly-typed
    /// <see cref="Description"/> model.
    /// </summary>
    public class JsonDescription
    {
        /// <summary>
        /// The manufacturer of the component or device.
        /// </summary>
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model designation of the component or device.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The serial number of the component or device.
        /// </summary>
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The station identifier of the component within a multi-station
        /// device.
        /// </summary>
        [JsonPropertyName("station")]
        public string Station { get; set; }

        /// <summary>
        /// The free-form description text.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDescription() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDescription"/>.
        /// </summary>
        public JsonDescription(IDescription description)
        {
            if (description != null)
            {
                Manufacturer = description.Manufacturer;
                Model = description.Model;
                SerialNumber = description.SerialNumber;
                Station = description.Station;
                Value = description.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IDescription"/>.
        /// </summary>
        public IDescription ToDescription()
        {
            var description = new Description();
            description.Manufacturer = Manufacturer;
            description.Model = Model;
            description.SerialNumber = SerialNumber;
            description.Station = Station;
            description.Value = Value;
            return description;
        }
    }
}