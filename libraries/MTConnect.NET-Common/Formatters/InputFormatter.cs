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
    public static class InputFormatter
    {
        private static readonly ConcurrentDictionary<string, IInputFormatter> _formatters = new ConcurrentDictionary<string, IInputFormatter>();
        private static bool _firstRead = true;


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