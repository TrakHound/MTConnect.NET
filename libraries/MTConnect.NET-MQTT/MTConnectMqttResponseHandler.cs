// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect
{
    /// <summary>
    /// Callback used by the MTConnect MQTT client to deliver a parsed response document back to
    /// application code, paired with the originating <see cref="IDevice"/>. The generic parameter
    /// <typeparamref name="TResponseDocument"/> is one of the MTConnect response document
    /// interfaces (devices, streams, assets); the handler is invoked once the client has
    /// reassembled the document from the relevant MQTT topics.
    /// </summary>
    /// <typeparam name="TResponseDocument">The MTConnect response document type carried to the handler.</typeparam>
    /// <param name="device">The device the document pertains to (may be null for agent-scoped documents).</param>
    /// <param name="responseDocument">The reassembled response document.</param>
    public delegate void MTConnectMqttResponseHandler<TResponseDocument>(IDevice device, TResponseDocument responseDocument);
}
