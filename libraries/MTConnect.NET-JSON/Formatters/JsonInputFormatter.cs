// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Input;
using MTConnect.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var bytes = new JsonDevice(device.Device).ToStream();
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
                var bytes = JsonFunctions.ConvertStream(JsonInputObservationGroup.Create(observations));
                if (bytes != null)
                {
                    return FormatWriteResult.Successful(bytes, ContentType);
                }
            }

            return FormatWriteResult.Error();
        }

        public FormatWriteResult Format(IEnumerable<IAssetInput> assets, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            if (!assets.IsNullOrEmpty())
            {
                var jsonAssetGroup = new JsonInputAssetGroup(assets);
                if (jsonAssetGroup != null)
                {
                    var bytes = JsonFunctions.ConvertStream(jsonAssetGroup);
                    if (bytes != null)
                    {
                        return FormatWriteResult.Successful(bytes, ContentType);
                    }
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

        public FormatReadResult<IEnumerable<IAsset>> CreateAssets(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            var messages = new List<string>();
            var warnings = new List<string>();
            var errors = new List<string>();

            IEnumerable<IAsset> assets = null;

            // Read Document
            if (content != null)
            {
                try
                {
                    var jsonAssetGroup = JsonSerializer.Deserialize<JsonInputAssetGroup>(content);
                    if (jsonAssetGroup != null)
                    {
                        assets = JsonInputAssetGroup.ToAssets(jsonAssetGroup);
                    }
                }
                catch { }
            }

            var success = assets != null;

            return new FormatReadResult<IEnumerable<IAsset>>(assets, success, messages, warnings, errors);
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