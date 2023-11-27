// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    public class JsonDestinationCollection
    {
        [JsonPropertyName("Destination")]
        public IEnumerable<JsonDestination> Destinations { get; set; }


        public JsonDestinationCollection() { }

        public JsonDestinationCollection(IEnumerable<IDestination> destinations)
        {
            if (!destinations.IsNullOrEmpty())
            {
                var jsonDestinations = new List<JsonDestination>();
                foreach (var destination in destinations)
                {
                    jsonDestinations.Add(new JsonDestination(destination));
                }
                Destinations = jsonDestinations;
            }
        }


        public IEnumerable<IDestination> ToDestinations()
        {
            if (!Destinations.IsNullOrEmpty())
            {
                var destinations = new List<IDestination>();

                foreach (var destination in Destinations)
                {
                    destinations.Add(destination.ToDestination());
                }

                return destinations;
            }

            return null;
        }
    }
}