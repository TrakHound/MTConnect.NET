// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.Files;
using MTConnect.Assets.Json.QIF;
using MTConnect.Assets.Json.RawMaterials;
using MTConnect.Assets.QIF;
using MTConnect.Assets.RawMaterials;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Input;
using MTConnect.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MTConnect.Formatters
{
    public class JsonInputFormatter : IInputFormatter
    {
        public string Id => "JSON";

        public string ContentType => "application/json";


        public FormatWriteResult Format(IDeviceInput device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                var bytes = new JsonDevice(device.Device).ToBytes();
                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IEnumerable<IObservationInput> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (!observations.IsNullOrEmpty())
            {
                var bytes = JsonFunctions.ConvertBytes(JsonInputObservationGroup.Create(observations));
                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IAssetInput asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                byte[] bytes;

                switch (asset.Type)
                {
                    case "CuttingTool": bytes = JsonFunctions.ConvertBytes(new JsonCuttingToolAsset(asset as CuttingToolAsset)); break;
                    case "File": bytes = JsonFunctions.ConvertBytes(new JsonFileAsset(asset as FileAsset)); break;
                    case "QIFDocumentWrapper": bytes = JsonFunctions.ConvertBytes(new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset)); break;
                    case "RawMaterial": bytes = JsonFunctions.ConvertBytes(new JsonRawMaterialAsset(asset as RawMaterialAsset)); break;

                    default: bytes = JsonFunctions.ConvertBytes(asset); break;
                }

                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }


        public FormatReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonDevice>(content);
                var entity = jsonEntity.ToDevice();
                var success = entity != null;

                return new FormatReadResult<IDevice>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IDevice>();
        }

        public FormatReadResult<IEnumerable<IObservationInput>> CreateObservations(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonObservationGroups = JsonSerializer.Deserialize<IEnumerable<JsonInputObservationGroup>>(content);
                var observationInputs = new List<IObservationInput>();
                foreach (var jsonObservationGroup in jsonObservationGroups.OrderBy(o => o.Timestamp))
                {
                    observationInputs.AddRange(jsonObservationGroup.ToObservationInputs());
                }

                var success = !observationInputs.IsNullOrEmpty();

                return new FormatReadResult<IEnumerable<IObservationInput>>(observationInputs, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IEnumerable<IObservationInput>>();
        }

        public FormatReadResult<IAsset> CreateAsset(string assetType, byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
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