// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDescription
    {
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("station")]
        public string Station { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonDescription() { }

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