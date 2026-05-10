// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Headers;
using MTConnect.Streams.Json;
using MTConnect.Streams.Output;
using System;

namespace MTConnect.NET_JSON_cppagent_Tests.TestHelpers
{
    /// <summary>
    /// Builds minimal Streams / Devices response documents tagged with
    /// the supplied configured version, then runs them through the
    /// JSON-cppagent envelope ctor under test.
    /// </summary>
    internal static class EnvelopeFixtures
    {
        public static JsonMTConnectStreams BuildStreamsEnvelope(Version configured)
        {
            var doc = new StreamsResponseOutputDocument
            {
                Header = new MTConnectStreamsHeader
                {
                    InstanceId = 1,
                    Version = configured.ToString(),
                    Sender = "test",
                },
                Streams = Array.Empty<IDeviceStreamOutput>(),
                Version = configured,
            };

            return new JsonMTConnectStreams(doc);
        }

        public static JsonMTConnectDevices BuildDevicesEnvelope(Version configured)
        {
            var doc = new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader
                {
                    InstanceId = 1,
                    Version = configured.ToString(),
                    Sender = "test",
                },
                Version = configured,
            };

            return new JsonMTConnectDevices(doc);
        }
    }
}
