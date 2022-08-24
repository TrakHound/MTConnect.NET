// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Formatters
{
    public static class PathFormatter
    {
        private static readonly ConcurrentDictionary<string, IPathFormatter> _formatters = new ConcurrentDictionary<string, IPathFormatter>();
        private static bool _firstRead = true;


        public static IEnumerable<string> GetDataItemIds(string documentFormatterId, IDevicesResponseDocument devicesResponseDocument, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Get the Formatter with the specified ID
                var formatter = GetFormatter(documentFormatterId);
                if (formatter != null)
                {
                    // Format the Response Document using the Formatter
                    return formatter.GetDataItemIds(devicesResponseDocument, path);
                }
            }

            return null;
        }


        private static IPathFormatter GetFormatter(string id)
        {
            if (_firstRead)
            {
                AddFormatters();
                _firstRead = false;
            }

            _formatters.TryGetValue(id, out var formatter);
            return formatter;
        }


        private static void AddFormatters()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var allTypes = assemblies.SelectMany(x => x.GetTypes());

                // Get IResponseDocumentFormatter Types
                var types = allTypes.Where(x => typeof(IPathFormatter).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                if (!types.IsNullOrEmpty())
                {
                    foreach (var type in types)
                    {
                        try
                        {
                            // Create new Instance of the Formatter and add to cached dictionary
                            var formatter = (IPathFormatter)Activator.CreateInstance(type);
                            _formatters.TryAdd(formatter.Id, formatter);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
