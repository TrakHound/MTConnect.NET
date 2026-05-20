// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Servers
{
    /// <summary>
    /// Carries the inputs of a <c>PUT</c>-style request that posts a single DataItem observation
    /// to the MTConnect agent over HTTP. The HTTP transport populates this from the request path
    /// and body and forwards it to the agent's observation buffer.
    /// </summary>
    public struct MTConnectObservationInputArgs
    {
        /// <summary>The device key (UUID or name) that owns the targeted DataItem.</summary>
        public string DeviceKey { get; set; }

        /// <summary>The DataItem identifier (<c>id</c>) or, where supported, its <c>name</c> that the observation applies to.</summary>
        public string DataItemKey { get; set; }

        /// <summary>The serialised observation value to record against the DataItem.</summary>
        public string Value { get; set; }
    }
}
