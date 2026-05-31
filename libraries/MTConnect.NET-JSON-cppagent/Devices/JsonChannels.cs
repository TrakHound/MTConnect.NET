// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of
    /// <see cref="JsonChannel"/> items inside a SensorConfiguration,
    /// keyed by the singular cppagent element name <c>Channel</c>.
    /// </summary>
    public class JsonChannels
    {
        /// <summary>
        /// The channel definitions in the container.
        /// </summary>
        [JsonPropertyName("Channel")]
        public IEnumerable<JsonChannel> Channels { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonChannels() { }

        /// <summary>
        /// Initializes the container from a channel sequence.
        /// </summary>
        public JsonChannels(IEnumerable<IChannel> channels)
        {
            if (!channels.IsNullOrEmpty())
            {
                var jsonChannels = new List<JsonChannel>();

                foreach (var channel in channels) jsonChannels.Add(new JsonChannel(channel));

                Channels = jsonChannels;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="IChannel"/> sequence.
        /// </summary>
        public IEnumerable<IChannel> ToChannels()
        {
            var channels = new List<IChannel>();

            if (!channels.IsNullOrEmpty())
            {
                foreach (var channel in Channels) channels.Add(channel.ToChannel());
            }

            return channels;
        }
    }
}