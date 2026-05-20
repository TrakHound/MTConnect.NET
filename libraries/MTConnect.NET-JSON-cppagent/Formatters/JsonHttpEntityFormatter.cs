// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.Json;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.Files;
using MTConnect.Assets.Json.QIF;
using MTConnect.Assets.Json.RawMaterials;
using MTConnect.Assets.RawMaterials;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MTConnect.Formatters
{
    /// <summary>
    /// HTTP entity formatter that serializes and deserializes individual
    /// MTConnect entities (Device, Asset, Composition, DataItem) using the
    /// cppagent-compatible JSON shape. Used by the PUT/POST entity-level
    /// endpoints that operate on a single entity rather than a full
    /// response document.
    /// </summary>
    public class JsonHttpEntityFormatter : IEntityFormatter
    {
        /// <summary>
        /// The formatter identifier exposed to the agent's content-type
        /// negotiation, distinguishing this formatter from the plain
        /// MTConnect JSON formatter.
        /// </summary>
        public virtual string Id => "JSON-cppagent";

        /// <summary>
        /// The HTTP <c>Content-Type</c> emitted by this formatter.
        /// </summary>
        public virtual string ContentType => "application/json";


        /// <summary>
        /// Serializes a single device using the cppagent-compatible
        /// <see cref="JsonDevice"/> surrogate, honouring the
        /// <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");

                var bytes = new JsonDevice(device).ToStream(indentOuput);
                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Single-observation serialization is not part of the cppagent
        /// entity surface, so this overload always returns an error
        /// result.
        /// </summary>
        public FormatWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Bulk-observation serialization is not part of the cppagent
        /// entity surface, so this overload always returns an error
        /// result.
        /// </summary>
        public FormatWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes a single asset by wrapping it in a one-element
        /// <see cref="JsonAssets"/> container and dispatching to the
        /// appropriate cppagent asset surrogate
        /// (<c>CuttingTool</c>, <c>File</c>, <c>RawMaterial</c>).
        /// </summary>
        public FormatWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                var assets = new JsonAssets();

                switch (asset.Type)
                {
                    case "CuttingTool":
                        assets.CuttingTools = new List<JsonCuttingToolAsset>();
                        assets.CuttingTools.Add(new JsonCuttingToolAsset(asset as CuttingToolAsset));
                        break;

                    case "File":
                        assets.Files = new List<JsonFileAsset>();
                        assets.Files.Add(new JsonFileAsset(asset as FileAsset));
                        break;

                    //case "QIFDocumentWrapper":
                    //    assets. = new List<JsonCuttingToolAsset>();
                    //    assets.CuttingTools.Add(new JsonCuttingToolAsset(asset as CuttingToolAsset));
                    //    bytes = JsonFunctions.ConvertBytes(new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset));
                    //    break;

                    case "RawMaterial":
                        assets.RawMaterials = new List<JsonRawMaterialAsset>();
                        assets.RawMaterials.Add(new JsonRawMaterialAsset(asset as RawMaterialAsset));
                        break;

                        //default:
                        //    bytes = JsonFunctions.ConvertBytes(asset);
                        //    break;
                }

                var bytes = JsonFunctions.ConvertStream(assets);
                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }


        /// <summary>
        /// Deserializes a single device from JSON via the
        /// <see cref="JsonDeviceContainer"/> envelope and reconstructs the
        /// strongly-typed model.
        /// </summary>
        public FormatReadResult<IDevice> CreateDevice(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonDeviceContainer>(content);
                var entity = jsonEntity.ToDevice();
                var success = entity != null;

                return new FormatReadResult<IDevice>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IDevice>();
        }

        /// <summary>
        /// Component-level deserialization is not yet implemented for the
        /// cppagent shape; the method always returns an empty result.
        /// </summary>
        public FormatReadResult<IComponent> CreateComponent(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                //var jsonEntity = JsonSerializer.Deserialize<JsonComponent>(content);
                //var entity = jsonEntity.ToComponent();
                //var success = entity != null;

                //return new FormattedEntityReadResult<IComponent>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IComponent>();
        }

        /// <summary>
        /// Deserializes a single composition from JSON via the
        /// <see cref="JsonComposition"/> surrogate and reconstructs the
        /// strongly-typed model.
        /// </summary>
        public FormatReadResult<IComposition> CreateComposition(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonComposition>(content);
                var entity = jsonEntity.ToComposition();
                var success = entity != null;

                return new FormatReadResult<IComposition>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IComposition>();
        }

        /// <summary>
        /// Deserializes a single data item from JSON via the
        /// <see cref="JsonDataItem"/> surrogate and reconstructs the
        /// strongly-typed model.
        /// </summary>
        public FormatReadResult<IDataItem> CreateDataItem(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonDataItem>(content);
                var entity = jsonEntity.ToDataItem();
                var success = entity != null;

                return new FormatReadResult<IDataItem>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IDataItem>();
        }

        /// <summary>
        /// Deserializes a single asset from JSON, dispatching to the
        /// appropriate cppagent asset surrogate
        /// (<c>CuttingTool</c>, <c>File</c>, <c>QIFDocumentWrapper</c>,
        /// <c>RawMaterial</c>) based on the supplied asset type
        /// discriminator. Any deserialization exception is captured into
        /// the <c>errors</c> list rather than propagated.
        /// </summary>
        public FormatReadResult<IAsset> CreateAsset(string assetType, Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            IAsset asset = null;

            // Read Document
            if (!string.IsNullOrEmpty(assetType) && content != null)
            {
                try
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        content.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    // Convert from UTF8 bytes
                    var json = Encoding.UTF8.GetString(bytes);
                    if (!string.IsNullOrEmpty(json))
                    {
                        switch (assetType)
                        {
                            case "CuttingTool":
                                asset = JsonSerializer.Deserialize<JsonCuttingToolAsset>(json).ToCuttingToolAsset();
                                break;

                            case "File":
                                asset = JsonSerializer.Deserialize<JsonFileAsset>(json).ToFileAsset();
                                break;

                            case "QIFDocumentWrapper":
                                asset = JsonSerializer.Deserialize<JsonQIFDocumentWrapperAsset>(json).ToQIFDocumentWrapperAsset();
                                break;

                            case "RawMaterial":
                                asset = JsonSerializer.Deserialize<JsonRawMaterialAsset>(json).ToRawMaterialAsset();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            var success = asset != null;

            return new FormatReadResult<IAsset>(asset, success, messages, warnings, errors);
        }


        private static T GetFormatterOption<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            if (!options.IsNullOrEmpty())
            {
                var x = options.FirstOrDefault(o => o.Key == key).Value;
                if (!string.IsNullOrEmpty(x))
                {
                    try
                    {
                        return (T)Convert.ChangeType(x, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }

        private static IEnumerable<T> GetFormatterOptions<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            var l = new List<T>();

            if (!options.IsNullOrEmpty())
            {
                var x = options.Where(o => o.Key == key);
                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x)
                    {
                        if (!string.IsNullOrEmpty(y.Value))
                        {
                            try
                            {
                                var obj = (T)Convert.ChangeType(y.Value, typeof(T));
                                l.Add(obj);
                            }
                            catch { }
                        }
                    }
                }
            }

            return l;
        }
    }
}