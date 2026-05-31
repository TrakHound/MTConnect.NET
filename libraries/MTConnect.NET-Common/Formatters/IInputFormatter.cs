// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Input;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    /// <summary>
    /// Serializes and deserializes adapter input payloads (device, observation, and asset inputs) for a specific wire format.
    /// </summary>
    public interface IInputFormatter
    {
        /// <summary>
        /// The unique identifier of the input format this formatter implements.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The MIME content type produced by this formatter.
        /// </summary>
        string ContentType { get; }


        /// <summary>
        /// Serializes a device input definition.
        /// </summary>
        /// <param name="device">The device input to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IDeviceInput device, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes a batch of observation inputs.
        /// </summary>
        /// <param name="observations">The observation inputs to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IEnumerable<IObservationInput> observations, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes a batch of asset inputs.
        /// </summary>
        /// <param name="assets">The asset inputs to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IEnumerable<IAssetInput> assets, IEnumerable<KeyValuePair<string, string>> options = null);


        /// <summary>
        /// Deserializes a device definition from the given payload.
        /// </summary>
        /// <param name="content">The serialized device payload.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes a batch of observation inputs from the given payload.
        /// </summary>
        /// <param name="content">The serialized observations payload.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IEnumerable<IObservationInput>> CreateObservations(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes a batch of assets from the given payload.
        /// </summary>
        /// <param name="content">The serialized assets payload.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IEnumerable<IAsset>> CreateAssets(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}