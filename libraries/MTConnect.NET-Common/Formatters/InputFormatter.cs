// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Extensions;
using MTConnect.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MTConnect.Formatters
{
    /// <summary>
    /// Static facade that discovers and caches the <see cref="IInputFormatter"/> implementations available across loaded assemblies and dispatches input serialization/deserialization to the one matching a requested format, timing each operation.
    /// </summary>
    public static class InputFormatter
    {
        private static readonly ConcurrentDictionary<string, IInputFormatter> _formatters = new ConcurrentDictionary<string, IInputFormatter>();
        private static bool _firstRead = true;


        /// <summary>
        /// Returns the MIME content type produced by the input formatter registered for the given format, or null when no such formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        public static string GetContentType(string documentFormatterId)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                return formatter.ContentType;
            }

            return null;
        }


        /// <summary>
        /// Serializes a device input using the formatter for the given format, returning an error result when no matching formatter exists. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        /// <param name="device">The device input to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, IDeviceInput device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                result = formatter.Format(device, options);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }

        /// <summary>
        /// Serializes a batch of observation inputs using the formatter for the given format, returning an error result when no matching formatter exists. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        /// <param name="observations">The observation inputs to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, IEnumerable<IObservationInput> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                result = formatter.Format(observations, options);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }

        /// <summary>
        /// Serializes a batch of asset inputs using the formatter for the given format, returning an error result when no matching formatter exists. The result's response duration is set to the elapsed time.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        /// <param name="assets">The asset inputs to serialize.</param>
        /// <param name="options">Optional format-specific key/value options.</param>
        public static FormatWriteResult Format(string documentFormatterId, IEnumerable<IAssetInput> assets, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                result = formatter.Format(assets, options);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }


        /// <summary>
        /// Deserializes a device from the given payload using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        /// <param name="content">The serialized device payload.</param>
        public static FormatReadResult<IDevice> CreateDevice(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateDevice(content);
            }

            return FormatReadResult<IDevice>.Error(null, $"Input Formatter Not found for \"{documentFormatterId}\"");
        }

        /// <summary>
        /// Deserializes a batch of observation inputs from the given payload using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        /// <param name="content">The serialized observations payload.</param>
        public static FormatReadResult<IEnumerable<IObservationInput>> CreateObservations(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateObservations(content);
            }

            return FormatReadResult<IEnumerable<IObservationInput>>.Error(null, $"Input Formatter Not found for \"{documentFormatterId}\"");
        }

        /// <summary>
        /// Deserializes a batch of assets from the given payload using the formatter for the given format; returns an error result when no matching formatter exists.
        /// </summary>
        /// <param name="documentFormatterId">The identifier of the input format.</param>
        /// <param name="content">The serialized assets payload.</param>
        public static FormatReadResult<IEnumerable<IAsset>> CreateAssets(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateAssets(content);
            }

            return FormatReadResult<IEnumerable<IAsset>>.Error(null, $"Input Formatter Not found for \"{documentFormatterId}\"");
        }


        private static IInputFormatter GetFormatter(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (_firstRead)
                {
                    AddFormatters();
                    _firstRead = false;
                }

                // Normalize the FormatterId
                var formatterId = id.ToLower();

                _formatters.TryGetValue(formatterId, out var formatter);
                return formatter;
            }

            return null;
        }


        private static void AddFormatters()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                // Get IEntityFormatter Types
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => typeof(IInputFormatter).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    foreach (var type in types)
                    {
                        try
                        {
                            // Create new Instance of the Formatter and add to cached dictionary
                            var formatter = (IInputFormatter)Activator.CreateInstance(type);

                            // Normalize the FormatterId
                            var formatterId = formatter.Id.ToLower();

                            _formatters.TryAdd(formatterId, formatter);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}