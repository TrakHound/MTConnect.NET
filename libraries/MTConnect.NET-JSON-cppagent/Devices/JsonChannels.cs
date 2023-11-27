// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonChannels
    {
        [JsonPropertyName("Channel")]
        public IEnumerable<JsonChannel> Channels { get; set; }


        public JsonChannels() { }

        public JsonChannels(IEnumerable<IChannel> channels)
        {
            if (!channels.IsNullOrEmpty())
            {
                var jsonChannels = new List<JsonChannel>();

                foreach (var channel in channels) jsonChannels.Add(new JsonChannel(channel));

                Channels = jsonChannels;
            }
        }


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