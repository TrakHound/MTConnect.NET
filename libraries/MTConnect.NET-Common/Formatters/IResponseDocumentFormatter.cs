// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.IO;

namespace MTConnect.Formatters
{
    /// <summary>
    /// Serializes and deserializes complete MTConnect response documents (Devices, Streams, Assets, and Error) for a specific document format.
    /// </summary>
    public interface IResponseDocumentFormatter
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
        /// Serializes a Devices response document.
        /// </summary>
        /// <param name="document">The Devices document to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes a Streams response document, passed by reference to allow streaming serializers to release it as it is consumed.
        /// </summary>
        /// <param name="document">The Streams output document to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes an Assets response document.
        /// </summary>
        /// <param name="document">The Assets document to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Serializes an Error response document.
        /// </summary>
        /// <param name="document">The Error document to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null);


        /// <summary>
        /// Deserializes a Devices response document from the given content stream.
        /// </summary>
        /// <param name="content">The serialized Devices document content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes a Streams response document from the given content stream.
        /// </summary>
        /// <param name="content">The serialized Streams document content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes an Assets response document from the given content stream.
        /// </summary>
        /// <param name="content">The serialized Assets document content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);

        /// <summary>
        /// Deserializes an Error response document from the given content stream.
        /// </summary>
        /// <param name="content">The serialized Error document content.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}