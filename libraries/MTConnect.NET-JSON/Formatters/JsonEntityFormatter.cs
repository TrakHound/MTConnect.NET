// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
using MTConnect.Observations;
using MTConnect.Streams.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MTConnect.Formatters
{
    public class JsonEntityFormatter : IEntityFormatter
    {
        public string Id => "JSON";

        public string ContentType => "application/json";


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

        public FormatWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (observation != null)
            {
                // Get Option for 'Category' output
                var categoryOutput = GetFormatterOption<bool>(options, "categoryOutput");

                // Get Option for 'InstanceId' output
                var instanceIdOutput = GetFormatterOption<bool>(options, "instanceIdOutput");

                Stream bytes = null;

                switch (observation.Category)
                {
                    // Sample
                    case DataItemCategory.SAMPLE:
                        var sampleObservation = SampleObservation.Create(observation);
                        if (sampleObservation != null)
                        {
                            bytes = JsonFunctions.ConvertStream(new JsonSample(sampleObservation, categoryOutput, instanceIdOutput));
                        }
                        break;

                    // Event
                    case DataItemCategory.EVENT:
                        var eventObservation = EventObservation.Create(observation);
                        if (eventObservation != null)
                        {
                            bytes = JsonFunctions.ConvertStream(new JsonEvent(eventObservation, categoryOutput, instanceIdOutput));
                        }
                        break;

                    // Condition
                    case DataItemCategory.CONDITION:
                        var conditionObservation = ConditionObservation.Create(observation);
                        if (conditionObservation != null)
                        {
                            bytes = JsonFunctions.ConvertStream(new JsonCondition(conditionObservation, categoryOutput, instanceIdOutput));
                        }
                        break;
                }

                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (!observations.IsNullOrEmpty())
            {
                // Get Option for 'Category' output
                var categoryOutput = GetFormatterOption<bool>(options, "categoryOutput");

                // Get Option for 'InstanceId' output
                var instanceIdOutput = GetFormatterOption<bool>(options, "instanceIdOutput");

                var x = new List<object>();

                foreach (var observation in observations)
                {
                    switch (observation.Category)
                    {
                        // Sample
                        case DataItemCategory.SAMPLE:
                            var sampleObservation = SampleObservation.Create(observation);
                            if (sampleObservation != null)
                            {
                                x.Add(new JsonSample(sampleObservation, categoryOutput, instanceIdOutput));
                            }
                            break;

                        // Event
                        case DataItemCategory.EVENT:
                            var eventObservation = EventObservation.Create(observation);
                            if (eventObservation != null)
                            {
                                x.Add(new JsonEvent(eventObservation, categoryOutput, instanceIdOutput));
                            }
                            break;

                        // Condition
                        case DataItemCategory.CONDITION:
                            var conditionObservation = ConditionObservation.Create(observation);
                            if (conditionObservation != null)
                            {
                                x.Add(new JsonCondition(conditionObservation, categoryOutput, instanceIdOutput));
                            }
                            break;
                    }

                }

                var bytes = JsonFunctions.ConvertStream(x);
                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                Stream bytes;

                switch (asset.Type)
                {
                    case "CuttingTool": bytes = JsonFunctions.ConvertStream(new JsonCuttingToolAsset(asset as CuttingToolAsset)); break;
                    case "File": bytes = JsonFunctions.ConvertStream(new JsonFileAsset(asset as FileAsset)); break;
                    case "QIFDocumentWrapper": bytes = JsonFunctions.ConvertStream(new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset)); break;
                    case "RawMaterial": bytes = JsonFunctions.ConvertStream(new JsonRawMaterialAsset(asset as RawMaterialAsset)); break;

                    default: bytes = JsonFunctions.ConvertStream(asset); break;
                }

                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }


        public FormatReadResult<IDevice> CreateDevice(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
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

        public FormatReadResult<IComponent> CreateComponent(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonComponent>(content);
                var entity = jsonEntity.ToComponent();
                var success = entity != null;

                return new FormatReadResult<IComponent>(entity, success, messages, warnings, errors);
            }
            catch { }

            return new FormatReadResult<IComponent>();
        }

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