// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Extensions;
using MTConnect.Observations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MTConnect.Formatters
{
    public static class EntityFormatter
    {
        private static readonly ConcurrentDictionary<string, IEntityFormatter> _formatters = new ConcurrentDictionary<string, IEntityFormatter>();
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


        public static FormatWriteResult Format(string documentFormatterId, IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
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

        public static FormatWriteResult Format(string documentFormatterId, IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                result = formatter.Format(observation, options);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }

        public static FormatWriteResult Format(string documentFormatterId, IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
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

        public static FormatWriteResult Format(string documentFormatterId, IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var stpw = Stopwatch.StartNew();

            FormatWriteResult result = FormatWriteResult.Error();

            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                result = formatter.Format(asset, options);
            }

            stpw.Stop();
            result.ResponseDuration = stpw.GetElapsedMilliseconds();

            return result;
        }


        public static FormatReadResult<IDevice> CreateDevice(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateDevice(content);
            }

            return FormatReadResult<IDevice>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormatReadResult<IComponent> CreateComponent(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateComponent(content);
            }

            return FormatReadResult<IComponent>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormatReadResult<IComposition> CreateComposition(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateComposition(content);
            }

            return FormatReadResult<IComposition>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormatReadResult<IDataItem> CreateDataItem(string documentFormatterId, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateDataItem(content);
            }

            return FormatReadResult<IDataItem>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormatReadResult<IAsset> CreateAsset(string documentFormatterId, string assetType, Stream content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateAsset(assetType, content);
            }

            return FormatReadResult<IAsset>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
        }


        private static IEntityFormatter GetFormatter(string id)
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
                            t => typeof(IEntityFormatter).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

                if (!types.IsNullOrEmpty())
                {
                    foreach (var type in types)
                    {
                        try
                        {
                            // Create new Instance of the Formatter and add to cached dictionary
                            var formatter = (IEntityFormatter)Activator.CreateInstance(type);

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