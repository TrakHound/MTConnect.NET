// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Collections.Concurrent;
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


        public static string Format(string documentFormatterId, IDevice device)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                return formatter.Format(device);
            }

            return null;
        }

        public static string Format(string documentFormatterId, IComponent component)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                return formatter.Format(component);
            }

            return null;
        }

        public static string Format(string documentFormatterId, IComposition composition)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                return formatter.Format(composition);
            }

            return null;
        }

        public static string Format(string documentFormatterId, IDataItem dataItem)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                return formatter.Format(dataItem);
            }

            return null;
        }

        public static string Format(string documentFormatterId, IObservation observation)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                return formatter.Format(observation);
            }

            return null;
        }

        public static string Format(string documentFormatterId, IAsset asset)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the string representation of the Entity using the Formatter
                return formatter.Format(asset);
            }

            return null;
        }


        public static FormattedEntityReadResult<IDevice> CreateDevice(string documentFormatterId, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateDevice(content);
            }

            return FormattedEntityReadResult<IDevice>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
        }

        public static FormattedEntityReadResult<IAsset> CreateAsset(string documentFormatterId, string assetType, byte[] content)
        {
            // Get the Formatter with the specified ID
            var formatter = GetFormatter(documentFormatterId);
            if (formatter != null)
            {
                // Create the Entity using the Formatter
                return formatter.CreateAsset(assetType, content);
            }

            return FormattedEntityReadResult<IAsset>.Error(null, $"Entity Formatter Not found for \"{documentFormatterId}\"");
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
