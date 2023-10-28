// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Files;
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
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MTConnect.Formatters
{
    public class JsonEntityFormatter : IEntityFormatter
    {
        public string Id => "JSON";

        public string ContentType => "application/json";


        public string Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (device != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");

                return new JsonDevice(device).ToString(indentOuput);
            }

            return null;
        }

        public string Format(IComponent component, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (component != null)
            {
                return new JsonComponent(component).ToString();
            }

            return null;
        }

        public string Format(IComposition composition, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (composition != null)
            {
                return new JsonComposition(composition).ToString();
            }

            return null;
        }

        public string Format(IDataItem dataItem, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (dataItem != null)
            {
                var indentOuput = GetFormatterOption<bool>(options, "indentOutput");

                return JsonFunctions.Convert(new JsonDataItem(dataItem), indented:indentOuput);
            }

            return null;
        }

        public string Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (observation != null)
            {
                // Get Option for 'Category' output
                var categoryOutput = GetFormatterOption<bool>(options, "categoryOutput");

                // Get Option for 'InstanceId' output
                var instanceIdOutput = GetFormatterOption<bool>(options, "instanceIdOutput");

                switch (observation.Category)
                {
                    // Sample
                    case Devices.DataItemCategory.SAMPLE:
                        var sampleObservation = SampleObservation.Create(observation);
                        if (sampleObservation != null)
                        {
                            return JsonFunctions.Convert(new JsonSample(sampleObservation, categoryOutput, instanceIdOutput));
                        }
                        break;

                    // Event
                    case Devices.DataItemCategory.EVENT:
                        var eventObservation = EventObservation.Create(observation);
                        if (eventObservation != null)
                        {
                            return JsonFunctions.Convert(new JsonEvent(eventObservation, categoryOutput, instanceIdOutput));
                        }
                        break;

                    // Condition
                    case Devices.DataItemCategory.CONDITION:
                        var conditionObservation = ConditionObservation.Create(observation);
                        if (conditionObservation != null)
                        {
                            return JsonFunctions.Convert(new JsonCondition(conditionObservation, categoryOutput, instanceIdOutput));
                        }
                        break;
                }
            }

            return null;
        }

        public string Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null)
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
                        case Devices.DataItemCategory.SAMPLE:
                            var sampleObservation = SampleObservation.Create(observation);
                            if (sampleObservation != null)
                            {
                                x.Add(new JsonSample(sampleObservation, categoryOutput, instanceIdOutput));
                            }
                            break;

                        // Event
                        case Devices.DataItemCategory.EVENT:
                            var eventObservation = EventObservation.Create(observation);
                            if (eventObservation != null)
                            {
                                x.Add(new JsonEvent(eventObservation, categoryOutput, instanceIdOutput));
                            }
                            break;

                        // Condition
                        case Devices.DataItemCategory.CONDITION:
                            var conditionObservation = ConditionObservation.Create(observation);
                            if (conditionObservation != null)
                            {
                                x.Add(new JsonCondition(conditionObservation, categoryOutput, instanceIdOutput));
                            }
                            break;
                    }

                }

                return JsonFunctions.Convert(x);
            }

            return null;
        }

        public string Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (asset != null)
            {
                switch (asset.Type)
                {
                    case "CuttingTool": return JsonFunctions.Convert(new JsonCuttingToolAsset(asset as CuttingTool));
                    case "File": return JsonFunctions.Convert(new JsonFileAsset(asset as File));
                    case "QIFDocumentWrapper": return JsonFunctions.Convert(new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset));
                    case "RawMaterial": return JsonFunctions.Convert(new JsonRawMaterialAsset(asset as RawMaterialAsset));

                    default: return JsonFunctions.Convert(asset);
                }
            }

            return null;
        }


        public FormattedEntityReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            try
            {
                var jsonEntity = JsonSerializer.Deserialize<JsonDevice>(content);
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
                var jsonEntity = JsonSerializer.Deserialize<JsonComponent>(content);
                var entity = jsonEntity.ToComponent();
                var success = entity != null;

                return new FormattedEntityReadResult<IComponent>(entity, success, messages, warnings, errors);
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