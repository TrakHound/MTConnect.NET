// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Formatters
{
    /// <summary>
    /// Serializes and deserializes individual MTConnect entities (devices, observations, assets, and device-model elements) for a specific document format.
    /// </summary>
    public interface IEntityFormatter
    {
        /// <summary>
        /// The unique identifier of the document format this formatter implements.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The MIME content type produced by this formatter.
        /// </summary>
        string ContentType { get; }


        /// <summary>
        /// Serializes a single device.
        /// </summary>
        /// <param name="device">The device to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes a single observation.
        /// </summary>
        /// <param name="observation">The observation to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes a batch of observations.
        /// </summary>
        /// <param name="observations">The observations to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes a single asset.
        /// </summary>
        /// <param name="asset">The asset to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null);


        /// <summary>
        /// Deserializes a device from the given content stream.
        /// </summary>
        /// <param name="content">The serialized device content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IDevice> CreateDevice(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes a component from the given content stream.
        /// </summary>
        /// <param name="content">The serialized component content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IComponent> CreateComponent(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes a composition from the given content stream.
        /// </summary>
        /// <param name="content">The serialized composition content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IComposition> CreateComposition(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes a data item from the given content stream.
        /// </summary>
        /// <param name="content">The serialized data item content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IDataItem> CreateDataItem(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes an asset of the given type from the given content stream.
        /// </summary>
        /// <param name="assetType">The MTConnect asset type to deserialize as.</param>
        /// <param name="content">The serialized asset content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IAsset> CreateAsset(string assetType, Stream content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}