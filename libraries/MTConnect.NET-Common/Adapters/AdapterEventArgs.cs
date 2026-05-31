// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Adapters
{
    /// <summary>
    /// Carries an adapter event payload together with the identifier of the
    /// client connection it relates to, so handlers can correlate the event
    /// with a specific consumer of the adapter's TCP stream.
    /// </summary>
    /// <typeparam name="T">The type of the event payload.</typeparam>
    public struct AdapterEventArgs<T>
    {
        /// <summary>
        /// The identifier of the client connection the event applies to. Empty
        /// or <c>null</c> when the event is not scoped to a single client.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The event payload.
        /// </summary>
        public T Data { get; set; }


        /// <summary>
        /// Initializes the event arguments with the originating client
        /// identifier and the payload.
        /// </summary>
        /// <param name="clientId">The client connection identifier.</param>
        /// <param name="data">The event payload.</param>
        public AdapterEventArgs(string clientId, T data)
        {
            ClientId = clientId;
            Data = data;
        }
    }
}