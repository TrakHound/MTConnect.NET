// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.Json;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.Files;
using MTConnect.Assets.Json.QIF;
using MTConnect.Assets.Json.RawMaterials;
using MTConnect.Assets.QIF;
using MTConnect.Assets.RawMaterials;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MTConnect.Formatters
{
    public class JsonHttpEntityFormatter : IEntityFormatter
    {
        public virtual string Id => "JSON-cppagent";

        public virtual string ContentType => "application/json";


        public FormattedEntityWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");

                var bytes = new JsonDevice(device).ToBytes(indentOuput);
                if (bytes != null)
                {
                    return FormattedEntityWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormattedEntityWriteResult.Error();
        }

        public FormattedEntityWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return FormattedEntityWriteResult.Error();
        }

        public FormattedEntityWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            return FormattedEntityWriteResult.Error();
        }

        public FormattedEntityWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
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

                //switch (asset.Type)
                //{
                //    case "CuttingTool": bytes = JsonFunctions.ConvertBytes(new JsonCuttingToolAsset(asset as CuttingToolAsset)); break;
                //    case "File": bytes = JsonFunctions.ConvertBytes(new JsonFileAsset(asset as FileAsset)); break;
                //    case "QIFDocumentWrapper": bytes = JsonFunctions.ConvertBytes(new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset)); break;
                //    case "RawMaterial": bytes = JsonFunctions.ConvertBytes(new JsonRawMaterialAsset(asset as RawMaterialAsset)); break;

                //    default: bytes = JsonFunctions.ConvertBytes(asset); break;
                //}

                var bytes = JsonFunctions.ConvertBytes(assets);
                if (bytes != null)
                {
                    return FormattedEntityWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormattedEntityWriteResult.Error();
        }


        public FormattedEntityReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonDeviceContainer>(content);
                var entity = jsonEntity.ToDevice();
                var success = entity != null;

                return new FormattedEntityReadResult<IDevice>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormattedEntityReadResult<IDevice>();
        }

        public FormattedEntityReadResult<IComponent> CreateComponent(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
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

            return new FormattedEntityReadResult<IComponent>();
        }

        public FormattedEntityReadResult<IComposition> CreateComposition(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonComposition>(content);
                var entity = jsonEntity.ToComposition();
                var success = entity != null;

                return new FormattedEntityReadResult<IComposition>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormattedEntityReadResult<IComposition>();
        }

        public FormattedEntityReadResult<IDataItem> CreateDataItem(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonDataItem>(content);
                var entity = jsonEntity.ToDataItem();
                var success = entity != null;

                return new FormattedEntityReadResult<IDataItem>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormattedEntityReadResult<IDataItem>();
        }

        public FormattedEntityReadResult<IAsset> CreateAsset(string assetType, byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
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
                    // Convert from UTF8 bytes
                    var json = Encoding.UTF8.GetString(content);
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

            return new FormattedEntityReadResult<IAsset>(asset, success, messages, warnings, errors);
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