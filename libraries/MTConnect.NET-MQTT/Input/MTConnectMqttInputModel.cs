using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttInputModel
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("observations")]
        public List<MTConnectMqttInputObservation> Observations { get; set; } = new List<MTConnectMqttInputObservation>();
        //public Dictionary<string, Dictionary<string, string>> Observations { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        //[JsonPropertyName("dataItems")]
        //public Dictionary<string, object> DataItems { get; set; }

        //[JsonPropertyName("messages")]
        //public Dictionary<string, MessageModel> Messages { get; set; }

        //[JsonPropertyName("conditions")]
        //public Dictionary<string, IEnumerable<ConditionModel>> Conditions { get; set; }

        //[JsonPropertyName("dataSets")]
        //public Dictionary<string, Dictionary<string, object>> DataSets { get; set; }

        //[JsonPropertyName("tables")]
        //public Dictionary<string, Dictionary<string, Dictionary<string, object>>> Tables { get; set; }


        public IEnumerable<IObservationInput> ToObservationInputs()
        {
            var observations = new List<IObservationInput>();
            observations.AddRange(ToObservationInputs(Timestamp, Observations));
            //observations.AddRange(ToObservationInputs(Timestamp, Messages));
            //observations.AddRange(ToObservationInputs(Timestamp, Conditions));
            //observations.AddRange(ToObservationInputs(Timestamp, DataSets));
            //observations.AddRange(ToObservationInputs(Timestamp, Tables));
            return observations;
        }

        private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, List<MTConnectMqttInputObservation> mqttObservations)
        {
            var observations = new List<IObservationInput>();

            if (!mqttObservations.IsNullOrEmpty())
            {
                var ts = timestamp.ToUnixTime();

                foreach (var mqttObservation in mqttObservations)
                {
                    if (!string.IsNullOrEmpty(mqttObservation.DataItemKey) && !mqttObservation.Values.IsNullOrEmpty())
                    {
                        var observation = new ObservationInput();
                        observation.DataItemKey = mqttObservation.DataItemKey;
                        observation.Timestamp = ts;

                        foreach (var value in mqttObservation.Values)
                        {
                            if (!string.IsNullOrEmpty(value.Key))
                            {
                                observation.AddValue(value.Key, value.Value);
                            }
                        }

                        observations.Add(observation);
                    }
                }
            }

            return observations;
        }

        //private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, Dictionary<string, Dictionary<string, string>> dataItems)
        //{
        //    var observations = new List<IObservationInput>();

        //    if (!dataItems.IsNullOrEmpty())
        //    {
        //        var ts = timestamp.ToUnixTime();

        //        foreach (var dataItem in dataItems)
        //        { 
        //            if (!string.IsNullOrEmpty(dataItem.Key) && !dataItem.Value.IsNullOrEmpty())
        //            {
        //                var observation = new ObservationInput();
        //                observation.DataItemKey = dataItem.Key;
        //                observation.Timestamp = ts;

        //                foreach (var value in dataItem.Value)
        //                {
        //                    if (!string.IsNullOrEmpty(value.Key))
        //                    {
        //                        observation.AddValue(value.Key, value.Value);
        //                    }
        //                }

        //                observations.Add(observation);
        //            }
        //        }
        //    }

        //    return observations;
        //}

        //private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, Dictionary<string, object> dataItems)
        //{
        //    var observations = new List<IObservationInput>();

        //    if (!dataItems.IsNullOrEmpty())
        //    {
        //        foreach (var dataItem in dataItems)
        //        {
        //            if (!string.IsNullOrEmpty(dataItem.Key))
        //            {
        //                var observation = new ObservationInput();
        //                observation.DataItemKey = dataItem.Key;
        //                observation.Timestamp = timestamp.ToUnixTime();
        //                observation.AddValue(ValueKeys.Result, dataItem.Value);
        //                observations.Add(observation);
        //            }
        //        }
        //    }

        //    return observations;
        //}

        //private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, Dictionary<string, MessageModel> messages)
        //{
        //    var observations = new List<IObservationInput>();

        //    if (!messages.IsNullOrEmpty())
        //    {
        //        foreach (var message in messages)
        //        {
        //            if (!string.IsNullOrEmpty(message.Key))
        //            {
        //                var observation = new ObservationInput();
        //                observation.DataItemKey = message.Key;
        //                observation.Timestamp = timestamp.ToUnixTime();
        //                observation.AddValue(ValueKeys.NativeCode, message.Value.NativeCode);
        //                observation.AddValue(ValueKeys.Result, message.Value.Message);
        //                observations.Add(observation);
        //            }
        //        }
        //    }

        //    return observations;
        //}

        //private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, Dictionary<string, IEnumerable<ConditionModel>> conditionModels)
        //{
        //    var observations = new List<IObservationInput>();

        //    if (!conditionModels.IsNullOrEmpty())
        //    {
        //        var ts = timestamp.ToUnixTime();

        //        foreach (var conditionModel in conditionModels)
        //        {
        //            if (!string.IsNullOrEmpty(conditionModel.Key))
        //            {
        //                var conditions = conditionModel.Value;
        //                if (!conditions.IsNullOrEmpty())
        //                {
        //                    foreach (var condition in conditions)
        //                    {
        //                        var observation = new ObservationInput();
        //                        observation.DataItemKey = conditionModel.Key;
        //                        observation.Timestamp = ts;
        //                        observation.AddValue(ValueKeys.Level, condition.Level);
        //                        observation.AddValue(ValueKeys.NativeCode, condition.NativeCode);
        //                        observation.AddValue(ValueKeys.NativeSeverity, condition.NativeSeverity);
        //                        observation.AddValue(ValueKeys.Qualifier, condition.Qualifier);
        //                        observation.AddValue(ValueKeys.Message, condition.Message);
        //                        observations.Add(observation);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return observations;
        //}

        //private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, Dictionary<string, Dictionary<string, object>> dataSetModels)
        //{
        //    var observations = new List<IObservationInput>();

        //    if (!dataSetModels.IsNullOrEmpty())
        //    {
        //        var ts = timestamp.ToUnixTime();

        //        foreach (var dataSetModel in dataSetModels)
        //        {
        //            if (!string.IsNullOrEmpty(dataSetModel.Key))
        //            {
        //                var entries = dataSetModel.Value;
        //                if (!entries.IsNullOrEmpty())
        //                {
        //                    var observation = new ObservationInput();
        //                    observation.DataItemKey = dataSetModel.Key;
        //                    observation.Timestamp = ts;

        //                    foreach (var entry in entries)
        //                    {
        //                        if (!string.IsNullOrEmpty(entry.Key))
        //                        {
        //                            observation.AddValue(ValueKeys.CreateDataSetValueKey(entry.Key), entry.Value);
        //                        }
        //                    }

        //                    observations.Add(observation);
        //                }
        //            }
        //        }
        //    }

        //    return observations;
        //}

        //private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, Dictionary<string, Dictionary<string, Dictionary<string, object>>> tableModels)
        //{
        //    var observations = new List<IObservationInput>();

        //    if (!tableModels.IsNullOrEmpty())
        //    {
        //        var ts = timestamp.ToUnixTime();

        //        foreach (var tableModel in tableModels)
        //        {
        //            if (!string.IsNullOrEmpty(tableModel.Key))
        //            {
        //                var entries = tableModel.Value;
        //                if (!entries.IsNullOrEmpty())
        //                {
        //                    var observation = new ObservationInput();
        //                    observation.DataItemKey = tableModel.Key;
        //                    observation.Timestamp = ts;

        //                    foreach (var entry in entries)
        //                    {
        //                        if (!string.IsNullOrEmpty(entry.Key))
        //                        {
        //                            var cells = entry.Value;
        //                            if (!cells.IsNullOrEmpty())
        //                            {
        //                                foreach (var cell in cells)
        //                                {
        //                                    observation.AddValue(ValueKeys.CreateTableValueKey(entry.Key, cell.Key), cell.Value);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    observations.Add(observation);
        //                }
        //            }
        //        }
        //    }

        //    return observations;
        //}
    }
}
