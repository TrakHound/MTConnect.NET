// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// The device files in the Agent's <c>devices</c> folder: listing, reading as a tree, applying changes
    /// with validation, and creating new device files.
    /// </summary>
    internal class DeviceFiles
    {
        private static readonly Regex _fileNameRegex = new Regex(@"^[A-Za-z0-9][A-Za-z0-9_.\- ]*\.xml$", RegexOptions.IgnoreCase);
        private static readonly HashSet<string> _categories = new HashSet<string> { "SAMPLE", "EVENT", "CONDITION" };

        private readonly string _path;
        private readonly bool _useLibraryReader;


        /// <summary>
        /// Creates a view of the device files at <paramref name="path"/> (a folder of <c>*.xml</c> files or a single file).
        /// </summary>
        /// <param name="path">The <c>devices</c> setting of the Agent (relative paths are relative to the Agent's folder).</param>
        /// <param name="useLibraryReader">Also check that the MTConnect.NET XML reader can read edited files (requires the XML formatter).</param>
        public DeviceFiles(string path, bool useLibraryReader = true)
        {
            path = string.IsNullOrEmpty(path) ? "devices" : path;
            _path = Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            _useLibraryReader = useLibraryReader;
        }

        /// <summary>True when the setting points at a folder, so new device files can be created in it.</summary>
        public bool IsFolder => Directory.Exists(_path);

        /// <summary>The full paths of the device files.</summary>
        public List<string> GetFilePaths()
        {
            if (Directory.Exists(_path)) return Directory.GetFiles(_path, "*.xml").OrderBy(o => o, StringComparer.OrdinalIgnoreCase).ToList();
            if (File.Exists(_path)) return new List<string> { _path };
            return new List<string>();
        }

        /// <summary>
        /// The full path of a listed device file by its file name; null when the name is not one of the files
        /// (so only files in the devices folder can be read or written).
        /// </summary>
        public string Resolve(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            return GetFilePaths().FirstOrDefault(p => string.Equals(Path.GetFileName(p), fileName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// A summary of every device file: its devices, or the error that stops it being read.
        /// </summary>
        public List<object> List()
        {
            return GetFilePaths().Select(path =>
            {
                try
                {
                    var document = DeviceDocument.Parse(File.ReadAllText(path));
                    return (object)new
                    {
                        file = Path.GetFileName(path),
                        devices = document.GetDevices().Select(d => new { id = (string)d.Attribute("id"), name = (string)d.Attribute("name"), uuid = (string)d.Attribute("uuid") }).ToList(),
                        error = (string)null
                    };
                }
                catch (Exception ex) when (ex is XmlException || ex is ConfigurationEditException)
                {
                    return new { file = Path.GetFileName(path), devices = new List<object>(), error = ex.Message };
                }
            }).ToList();
        }

        /// <summary>
        /// The tree of a device file for the form.
        /// </summary>
        public object GetTree(string path)
        {
            var text = File.ReadAllText(path);
            var document = DeviceDocument.Parse(text);
            return new
            {
                file = Path.GetFileName(path),
                version = document.Version,
                devices = document.GetDevices().Select(document.BuildNode).ToList()
            };
        }

        /// <summary>
        /// Applies operations to a file's text and validates the result. Returns the new text.
        /// </summary>
        public string Apply(string path, string text, IEnumerable<DeviceOperation> operations)
        {
            var result = DeviceDocument.ApplyOperations(text, operations);
            Validate(result, path);
            return result;
        }

        /// <summary>
        /// Creates the text of a new device file with the data items every device needs.
        /// </summary>
        public static string CreateDeviceText(string id, string name, string uuid)
        {
            string A(string value) => DeviceDocument.EscapeAttribute(value);
            var nl = Environment.NewLine;
            return
                "<Device id=\"" + A(id) + "\" name=\"" + A(name) + "\" uuid=\"" + A(uuid) + "\">" + nl +
                "\t<DataItems>" + nl +
                "\t\t<DataItem category=\"EVENT\" id=\"" + A(id) + "_avail\" type=\"AVAILABILITY\"/>" + nl +
                "\t\t<DataItem category=\"EVENT\" id=\"" + A(id) + "_asset_chg\" type=\"ASSET_CHANGED\"/>" + nl +
                "\t\t<DataItem category=\"EVENT\" id=\"" + A(id) + "_asset_rem\" type=\"ASSET_REMOVED\"/>" + nl +
                "\t</DataItems>" + nl +
                "</Device>" + nl;
        }

        /// <summary>
        /// Validates the request for a new device file and returns its full path and text.
        /// </summary>
        public string PrepareNewFile(string fileName, string name, string uuid, out string text)
        {
            if (!IsFolder) throw new ConfigurationEditException("New device files can only be created when 'devices' is a folder");
            if (string.IsNullOrWhiteSpace(name)) throw new ConfigurationEditException("A device name is required");

            fileName = string.IsNullOrWhiteSpace(fileName) ? name.Trim() : fileName.Trim();
            if (!fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) fileName += ".xml";
            if (!_fileNameRegex.IsMatch(fileName)) throw new ConfigurationEditException("The file name may only contain letters, digits, spaces, '.', '-' and '_'");

            var path = Path.Combine(_path, fileName);
            if (File.Exists(path)) throw new ConfigurationEditException($"A file named '{fileName}' already exists");

            var id = Regex.Replace(name.Trim(), @"[^A-Za-z0-9_.\-]", "_");
            if (!Regex.IsMatch(id, "^[A-Za-z_]")) id = "d_" + id;
            text = CreateDeviceText(id, name.Trim(), string.IsNullOrWhiteSpace(uuid) ? Guid.NewGuid().ToString() : uuid.Trim());

            Validate(text, path);
            return path;
        }

        /// <summary>
        /// Checks a device file: well-formed XML with at least one device; required attributes; unique ids;
        /// valid data item categories; device names and UUIDs not used by other device files; and (when enabled)
        /// readable by the MTConnect.NET XML reader. Throws <see cref="ConfigurationEditException"/>.
        /// </summary>
        public void Validate(string text, string path)
        {
            DeviceDocument document;
            try { document = DeviceDocument.Parse(text); }
            catch (XmlException ex) { throw new ConfigurationEditException("The XML is not well formed: " + ex.Message); }

            var devices = document.GetDevices();
            if (devices.Count == 0) throw new ConfigurationEditException("The file must contain a Device");

            var errors = new List<string>();
            var ids = new HashSet<string>();
            foreach (var device in devices)
            {
                foreach (var element in device.DescendantsAndSelf())
                {
                    var kind = DeviceDocument.GetKind(element);
                    var id = (string)element.Attribute("id");

                    // Every id (including coordinate systems, motions and specifications) must be unique
                    if (!string.IsNullOrEmpty(id) && !ids.Add(id)) errors.Add($"The id '{id}' is used more than once");
                    if (kind == "other") continue;

                    var label = $"{element.Name.LocalName}{(string.IsNullOrEmpty(id) ? "" : " '" + id + "'")}";
                    if (string.IsNullOrEmpty(id)) errors.Add($"{label} has no id");

                    if (kind == "device")
                    {
                        if (string.IsNullOrEmpty((string)element.Attribute("name"))) errors.Add($"{label} has no name");
                        if (string.IsNullOrEmpty((string)element.Attribute("uuid"))) errors.Add($"{label} has no uuid");
                    }
                    else if (kind == "dataItem")
                    {
                        if (string.IsNullOrEmpty((string)element.Attribute("type"))) errors.Add($"{label} has no type");
                        var category = (string)element.Attribute("category");
                        if (!_categories.Contains(category ?? "")) errors.Add($"{label} has an invalid category '{category}' (SAMPLE, EVENT or CONDITION)");
                    }
                }
            }

            // Device names and UUIDs must be unique across all device files
            foreach (var otherPath in GetFilePaths().Where(p => !string.Equals(Path.GetFullPath(p), Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    foreach (var other in DeviceDocument.Parse(File.ReadAllText(otherPath)).GetDevices())
                    {
                        foreach (var device in devices)
                        {
                            if ((string)device.Attribute("uuid") == (string)other.Attribute("uuid")) errors.Add($"The uuid '{(string)device.Attribute("uuid")}' is already used in {Path.GetFileName(otherPath)}");
                            if ((string)device.Attribute("name") == (string)other.Attribute("name")) errors.Add($"The device name '{(string)device.Attribute("name")}' is already used in {Path.GetFileName(otherPath)}");
                        }
                    }
                }
                catch { }
            }

            if (errors.Count > 0) throw new ConfigurationEditException(string.Join("; ", errors.Distinct().Take(10)));

            if (_useLibraryReader && ReadWithLibrary(text) < devices.Count)
            {
                throw new ConfigurationEditException("The MTConnect.NET XML reader could not read every device in the file");
            }
        }

        /// <summary>The number of devices the Agent's own XML reader reads from the text (as the Agent loads device files).</summary>
        private static int ReadWithLibrary(string text)
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                {
                    var document = ResponseDocumentFormatter.CreateDevicesResponseDocument(DocumentFormat.XML, stream).Content;
                    if (document?.Devices != null && document.Devices.Any()) return document.Devices.Count();
                }

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                {
                    return EntityFormatter.CreateDevice(DocumentFormat.XML, stream).Content != null ? 1 : 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
