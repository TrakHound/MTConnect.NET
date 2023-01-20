// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Files
{
    public class JsonDestination
    {
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }


        public JsonDestination() { }

        public JsonDestination(Destination destination)
        {
            if (destination != null)
            {
                DeviceUuid = destination.DeviceUuid;
            }
        }


        public Destination ToDestination()
        {
            var destination = new Destination();
            destination.DeviceUuid = DeviceUuid;
            return destination;
        }
    }
}