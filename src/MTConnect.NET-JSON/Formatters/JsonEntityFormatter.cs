// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
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


        public string Format(IDevice device)
        {
            if (device != null)
            {
                return new JsonDevice(device).ToString();
            }

            return null;
        }

        public string Format(IComponent component)
        {
            if (component != null)
            {
                return new JsonComponent(component).ToString();
            }

            return null;
        }

        public string Format(IComposition composition)
        {
            if (composition != null)
            {
                return new JsonComposition(composition).ToString();
            }

            return null;
        }

        public string Format(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                return new JsonDataItem(dataItem).ToString();
            }

            return null;
        }

        public string Format(IObservation observation)
        {
            if (observation != null)
            {
                switch (observation.Category)
                {
                    // Sample
                    case Devices.DataItems.DataItemCategory.SAMPLE:
                        var sampleObservation = SampleObservation.Create(observation);
                        if (sampleObservation != null)
                        {
                            return JsonFunctions.Convert(new JsonSample(sampleObservation));
                        }
                        break;

                    // Event
                    case Devices.DataItems.DataItemCategory.EVENT:
                        var eventObservation = EventObservation.Create(observation);
                        if (eventObservation != null)
                        {
                            return JsonFunctions.Convert(new JsonEvent(eventObservation));
                        }
                        break;

                    // Condition
                    case Devices.DataItems.DataItemCategory.CONDITION:
                        var conditionObservation = ConditionObservation.Create(observation);
                        if (conditionObservation != null)
                        {
                            return JsonFunctions.Convert(new JsonCondition(conditionObservation));
                        }
                        break;
                }
            }

            return null;
        }

        public string Format(IEnumerable<IObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var x = new List<object>();

                foreach (var observation in observations)
                {
                    switch (observation.Category)
                    {
                        // Sample
                        case Devices.DataItems.DataItemCategory.SAMPLE:
                            var sampleObservation = SampleObservation.Create(observation);
                            if (sampleObservation != null)
                            {
                                x.Add(new JsonSample(sampleObservation));
                            }
                            break;

                        // Event
                        case Devices.DataItems.DataItemCategory.EVENT:
                            var eventObservation = EventObservation.Create(observation);
                            if (eventObservation != null)
                            {
                                x.Add(new JsonEvent(eventObservation));
                            }
                            break;

                        // Condition
                        case Devices.DataItems.DataItemCategory.CONDITION:
                            var conditionObservation = ConditionObservation.Create(observation);
                            if (conditionObservation != null)
                            {
                                x.Add(new JsonCondition(conditionObservation));
                            }
                            break;
                    }

                }

                return JsonFunctions.Convert(x);
            }

            return null;
        }

        public string Format(IAsset asset)
        {
            if (asset != null)
            {
                switch (asset.Type)
                {
                    case "CuttingTool": return JsonFunctions.Convert(new JsonCuttingToolAsset(asset as CuttingToolAsset));
                    case "File": return JsonFunctions.Convert(new JsonFileAsset(asset as FileAsset));
                    case "QIFDocumentWrapper": return JsonFunctions.Convert(new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset));
                    case "RawMaterial": return JsonFunctions.Convert(new JsonRawMaterialAsset(asset as RawMaterialAsset));

                    default: return JsonFunctions.Convert(asset);
                }
            }

            return null;
        }


        public FormattedEntityReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            //var messages = new List<string>();
            //var warnings = new List<string>();
            //var errors = new List<string>();

            //// Read Document
            //var entity = JsonDevice.FromJson(content);
            //var success = entity != null;

            //return new FormattedEntityReadResult<IDevice>(entity, success, messages, warnings, errors);

            return new FormattedEntityReadResult<IDevice>();
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
