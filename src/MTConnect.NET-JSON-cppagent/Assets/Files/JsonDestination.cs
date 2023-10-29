// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    public class JsonDestination
    {
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }


        public JsonDestination() { }

        public JsonDestination(IDestination destination)
        {
            if (destination != null)
            {
                DeviceUuid = destination.DeviceUuid;
            }
        }


        public IDestination ToDestination()
        {
            var destination = new Destination();
            destination.DeviceUuid = DeviceUuid;
            return destination;
        }
    }
}