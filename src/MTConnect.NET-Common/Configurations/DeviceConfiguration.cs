// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Configurations
{
    /// <summary>
    /// An MTConnect Device Information Model read from a configuration file (ex. devices.xml)
    /// </summary>
    public class DeviceConfiguration : Device
    {
        public const string DefaultFilename = "devices.xml";


        /// <summary>
        /// The File Path that the configuration was read from
        /// </summary>
        public string Path { get; set; }


        public DeviceConfiguration() { }

        public DeviceConfiguration(IDevice device, string path = null)
        {
            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                Uuid = device.Uuid;
                Type = device.Type;
                Iso841Class = device.Iso841Class;
                NativeName = device.NativeName;
                SampleInterval = device.SampleInterval;
                SampleRate = device.SampleRate;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                MTConnectVersion = device.MTConnectVersion;
                Description = device.Description;
                Configuration = device.Configuration;
                DataItems = device.DataItems;
                Components = device.Components;
                Compositions = device.Compositions;
                References = device.References;

                Path = path;
            }
        }


        /// <summary>
        /// Gets a list of Devices from the specified file (ex. devices.xml)
        /// </summary>
        /// <param name="filePath">The path to the Device Configuration file</param>
        public static IEnumerable<DeviceConfiguration> FromFile(string filePath, string documentFormatterId)
        {
            // Set the Filename
            var path = !string.IsNullOrEmpty(filePath) ? filePath : DefaultFilename;

            // Add Working directory (if path is not rooted)
            var rootPath = path;
            if (!System.IO.Path.IsPathRooted(rootPath))
            {
                rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            if (!string.IsNullOrEmpty(rootPath))
            {
                try
                {
                    if (File.Exists(rootPath))
                    {
                        var contents = File.ReadAllBytes(rootPath);
                        if (contents != null)
                        {
                            // Read ResponseDocument Format
                            var devicesDocument = Formatters.ResponseDocumentFormatter.CreateDevicesResponseDocument(documentFormatterId, contents).Document;
                            if (devicesDocument != null && devicesDocument.Devices != null && devicesDocument.Devices.Count() > 0)
                            {
                                var devices = new List<DeviceConfiguration>();

                                foreach (var device in devicesDocument.Devices)
                                {
                                    devices.Add(new DeviceConfiguration(device, rootPath));
                                }

                                return devices;
                            }
                            else
                            {
                                // Read Single Entity Format
                                var device = Formatters.EntityFormatter.CreateDevice(documentFormatterId, contents).Entity;
                                if (device != null)
                                {
                                    return new List<DeviceConfiguration> { new DeviceConfiguration(device, rootPath) };
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

#if NETCOREAPP1_0_OR_GREATER

        /// <summary>
        /// Gets a list of Devices from the specified file (ex. devices.xml)
        /// </summary>
        /// <param name="filePath">The path to the Device Configuration file</param>
        public static async Task<IEnumerable<DeviceConfiguration>> FromFileAsync(string filePath, string documentFormatterId)
        {
            // Set the Filename
            var path = !string.IsNullOrEmpty(filePath) ? filePath : DefaultFilename;

            // Add Working directory (if path is not rooted)
            var rootPath = path;
            if (!System.IO.Path.IsPathRooted(rootPath))
            {
                rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            if (!string.IsNullOrEmpty(rootPath))
            {
                try
                {
                    if (File.Exists(rootPath))
                    {
                        var contents = await File.ReadAllBytesAsync(rootPath);
                        if (contents != null)
                        {
                            // Read ResponseDocument Format
                            var devicesDocument = Formatters.ResponseDocumentFormatter.CreateDevicesResponseDocument(documentFormatterId, contents).Document;
                            if (devicesDocument != null && devicesDocument.Devices != null && devicesDocument.Devices.Count() > 0)
                            {
                                var devices = new List<DeviceConfiguration>();

                                foreach (var device in devicesDocument.Devices)
                                {
                                    devices.Add(new DeviceConfiguration(device, rootPath));
                                }

                                return devices;
                            }
                            else
                            {
                                // Read Single Entity Format
                                var device = Formatters.EntityFormatter.CreateDevice(documentFormatterId, contents).Entity;
                                if (device != null)
                                {
                                    return new List<DeviceConfiguration> { new DeviceConfiguration(device, rootPath) };
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

#endif

        /// <summary>
        /// Gets a list of Devices from files (ex. devices.xml) found in the specified directory path
        /// </summary>
        /// <param name="dirPath">The path to the directory containing Device Configuration files</param>
        public static IEnumerable<DeviceConfiguration> FromFiles(string path, string documentFormatterId)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Add Working directory (if path is not rooted)
                var rootPath = path;
                if (!System.IO.Path.IsPathRooted(rootPath))
                {
                    rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                try
                {
                    if (Directory.Exists(rootPath))
                    {
                        var files = Directory.GetFiles(rootPath, "*.xml");
                        if (files != null && files.Length > 0)
                        {
                            var devices = new List<DeviceConfiguration>();

                            foreach (var file in files)
                            {
                                var documentDevices = FromFile(file, documentFormatterId);
                                if (!documentDevices.IsNullOrEmpty())
                                {
                                    foreach (var device in documentDevices)
                                    {
                                        devices.Add(device);
                                    }
                                }
                            }

                            return devices;
                        }
                    }
                    else
                    {
                        return FromFile(rootPath, documentFormatterId);
                    }
                }
                catch { }
            }

            return null;
        }

#if NETCOREAPP1_0_OR_GREATER

        /// <summary>
        /// Gets a list of Devices from files (ex. devices.xml) found in the specified directory path
        /// </summary>
        /// <param name="dirPath">The path to the directory containing Device Configuration files</param>
        public static async Task<IEnumerable<DeviceConfiguration>> FromFilesAsync(string path, string documentFormatterId)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Add Working directory (if path is not rooted)
                var rootPath = path;
                if (!System.IO.Path.IsPathRooted(rootPath))
                {
                    rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                try
                {
                    if (Directory.Exists(rootPath))
                    {
                        var files = Directory.GetFiles(rootPath, "*.xml");
                        if (files != null && files.Length > 0)
                        {
                            var devices = new List<DeviceConfiguration>();

                            foreach (var file in files)
                            {
                                var documentDevices = await FromFileAsync(file, documentFormatterId);
                                if (!documentDevices.IsNullOrEmpty())
                                {
                                    foreach (var device in documentDevices)
                                    {
                                        devices.Add(device);
                                    }
                                }
                            }

                            return devices;
                        }
                    }
                    else
                    {
                        return await FromFileAsync(rootPath, documentFormatterId);
                    }
                }
                catch { }
            }

            return null;
        }

#endif

    }
}