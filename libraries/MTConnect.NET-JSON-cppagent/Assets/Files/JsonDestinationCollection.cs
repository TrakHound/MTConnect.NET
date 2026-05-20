// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for the typed container of
    /// <see cref="JsonDestination"/> entries attached to a File asset,
    /// keyed by the singular cppagent element name <c>Destination</c>.
    /// </summary>
    public class JsonDestinationCollection
    {
        /// <summary>
        /// The destinations in the container.
        /// </summary>
        [JsonPropertyName("Destination")]
        public IEnumerable<JsonDestination> Destinations { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDestinationCollection() { }

        /// <summary>
        /// Initializes the container from a destination sequence.
        /// </summary>
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


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="IDestination"/> sequence.
        /// </summary>
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