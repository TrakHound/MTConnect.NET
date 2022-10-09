// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Output;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace MTConnect.Streams.Xml
{
    internal static class XmlObservation
    {
        public const string DataSetSuffix = "DataSet";
        public const string TableSuffix = "Table";
        public const string TimeSeriesSuffix = "TimeSeries";
        public const string XmlAttributeName = "XmlAttributeAttribute";

        private const string ConditionUnavailable = "Unavailable";
        private const string ConditionFault = "Fault";
        private const string ConditionWarning = "Warning";
        private const string ConditionNormal = "Normal";

        private const string ResetTriggeredNotSpecified = "NOT_SPECIFIED";


        public static bool IsDataSet(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(DataSetSuffix);
        }

        public static bool IsTable(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(TableSuffix);
        }

        public static bool IsTimeSeries(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(TimeSeriesSuffix);
        }

        public static string GetDataItemType(string elementName)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                var name = elementName;

                if (IsDataSet(name) && name.Length > DataSetSuffix.Length)
                {
                    // Remove the "DataSet" suffix from the Type
                    name = name.Substring(0, name.Length - DataSetSuffix.Length);
                }
                else if (IsTable(name) && name.Length > TableSuffix.Length)
                {
                    // Remove the "Table" suffix from the Type
                    name = name.Substring(0, name.Length - TableSuffix.Length);
                }
                else if (IsTimeSeries(name) && name.Length > TimeSeriesSuffix.Length)
                {
                    // Remove the "TimeSeries" suffix from the Type
                    name = name.Substring(0, name.Length - TimeSeriesSuffix.Length);
                }

                return DataItem.GetPascalCaseType(name);
            }

            return null;
        }

        public static DataItemRepresentation GetRepresentation(string elementName)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                if (elementName.EndsWith(DataSetSuffix))
                {
                    return DataItemRepresentation.DATA_SET;
                }
                else if (elementName.EndsWith(TableSuffix))
                {
                    return DataItemRepresentation.TABLE;
                }
                else if (elementName.EndsWith(TimeSeriesSuffix))
                {
                    return DataItemRepresentation.TIME_SERIES;
                }
            }

            return DataItemRepresentation.VALUE;
        }


        public static void WriteXml(
            XmlWriter writer,
            IObservation observation,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            bool outputComments = false
            )
        {
            try
            {
                // Get Element name
                var name = GetElementName(observation);
                var prefix = GetNamespacePrefix(observation.Type);

                if (!string.IsNullOrEmpty(name))
                {
                    // Add Comments
                    if (outputComments && observation.DataItem != null)
                    {
                        // Write DataItem Type Description as Comment
                        if (!string.IsNullOrEmpty(observation.DataItem.TypeDescription))
                        {
                            writer.WriteComment($"Type = {observation.DataItem.Type} : {observation.DataItem.TypeDescription}");
                        }

                        // Write DataItem SubType Description as Comment
                        if (!string.IsNullOrEmpty(observation.DataItem.SubType) && !string.IsNullOrEmpty(observation.DataItem.SubTypeDescription))
                        {
                            writer.WriteComment($"SubType = {observation.DataItem.SubType} : {observation.DataItem.SubTypeDescription}");
                        }

                        // Write Result Value Description
                        var result = observation.GetValue(ValueKeys.Result);
                        if (result != null)
                        {
                            var resultDescription = Observation.GetDescriptionText(observation.Category, observation.Type, observation.SubType, result);
                            if (!string.IsNullOrEmpty(resultDescription))
                            {
                                writer.WriteComment($"Result = {result} : {resultDescription}");
                            }
                        }
                    }

                    // Write Element Name
                    if (!string.IsNullOrEmpty(prefix) && !extendedSchemas.IsNullOrEmpty())
                    {
                        var ns = extendedSchemas.FirstOrDefault(o => o.Alias == prefix);
                        if (ns != null && ns.Urn != null)
                        {
                            // Write Element Name with Prefix and Namespace
                            writer.WriteStartElement(prefix, name, ns.Urn);
                        }
                        else
                        {
                            // Write namespace as 'dummy'. This will be incorrect but won't throw an exception on the XmlWriter
                            writer.WriteStartElement(prefix, name, "dummy");
                        }
                    }
                    else writer.WriteStartElement(name);

                    // DataItemId
                    writer.WriteAttributeString("dataItemId", observation.DataItemId);

                    // Name
                    if (!string.IsNullOrEmpty(observation.Name))
                    {
                        writer.WriteAttributeString("name", observation.Name);
                    }

                    // Type
                    if (!string.IsNullOrEmpty(observation.Type) && observation.Category == DataItemCategory.CONDITION)
                    {
                        writer.WriteAttributeString("type", observation.Type);
                    }

                    // SubType
                    if (!string.IsNullOrEmpty(observation.SubType))
                    {
                        writer.WriteAttributeString("subType", observation.SubType);
                    }

                    // CompositionId
                    if (!string.IsNullOrEmpty(observation.CompositionId))
                    {
                        writer.WriteAttributeString("compositionId", observation.CompositionId);
                    }

                    // Sequence
                    writer.WriteAttributeString("sequence", observation.Sequence.ToString());

                    // Timestamp
                    writer.WriteAttributeString("timestamp", observation.Timestamp.ToString("o"));


                    // Add Values
                    if (!observation.Values.IsNullOrEmpty())
                    {
                        var values = observation.Values.ToList();

                        // Write Common Values
                        for (var i = 0; i < values.Count(); i++)
                        {
                            WriteCommonValuesXml(writer, observation, values[i]);
                        }

                        if (observation.Category == DataItemCategory.CONDITION)
                        {
                            // Set InnerText to the Message (Condition)
                            var message = observation.GetValue(ValueKeys.Message);
                            if (!string.IsNullOrEmpty(message)) writer.WriteString(message);
                        }
                        else
                        {
                            // Set InnerText to the Result
                            var result = observation.GetValue(ValueKeys.Result);
                            if (!string.IsNullOrEmpty(result)) writer.WriteString(result);
                        }

                        // Write Representation specific Values
                        switch (observation.Representation)
                        {
                            case DataItemRepresentation.DATA_SET: WriteDataSetXml(writer, observation); break;
                            case DataItemRepresentation.TABLE: WriteTableXml(writer, observation); break;
                            case DataItemRepresentation.TIME_SERIES: WriteTimeSeriesXml(writer, observation); break;
                        }
                    }

                    writer.WriteEndElement();
                }
            }
            catch { }
        }


        private static string GetElementName(IObservation observation)
        {
            if (observation != null && !string.IsNullOrEmpty(observation.Type))
            {
                if (observation.Category == DataItemCategory.CONDITION)
                {
                    // Set Element Name to Condition Level
                    // (Using Constants is faster than ToPascalCase() method)
                    switch (observation.GetValue(ValueKeys.Level))
                    {
                        case "FAULT": return ConditionFault;
                        case "WARNING": return ConditionWarning;
                        case "NORMAL": return ConditionNormal;
                        default: return ConditionUnavailable;
                    }
                }
                else
                {
                    var type = observation.Type;
                    type = RemoveNamespacePrefix(type);

                    var elementName = XmlObservation.GetDataItemType(type);

                    // Add Suffix based on Representation
                    switch (observation.Representation)
                    {
                        case DataItemRepresentation.DATA_SET: elementName += XmlObservation.DataSetSuffix; break;
                        case DataItemRepresentation.TABLE: elementName += XmlObservation.TableSuffix; break;
                        case DataItemRepresentation.TIME_SERIES: elementName += XmlObservation.TimeSeriesSuffix; break;
                    }

                    return elementName;
                }
            }

            return null;
        }

        private static string GetNamespacePrefix(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Contains(':'))
                {
                    return type.Substring(0, type.IndexOf(':'));
                }
            }

            return null;
        }

        private static string RemoveNamespacePrefix(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Contains(':'))
                {
                    return type.Substring(type.IndexOf(':') + 1);
                }

                return type;
            }

            return null;
        }


        private static void WriteCommonValuesXml(XmlWriter writer, IObservation observation, ObservationValue observationValue)
        {
            if (!string.IsNullOrEmpty(observationValue.Key) && !ValueKeys.IsDataSetKey(observationValue.Key) && !ValueKeys.IsTableKey(observationValue.Key) && !ValueKeys.IsTimeSeriesKey(observationValue.Key))
            {
                switch (observationValue.Key)
                {
                    // Count
                    case ValueKeys.Count:
                        writer.WriteAttributeString("count", observationValue.Value);
                        break;

                    // Duration
                    case ValueKeys.Duration:
                        if (observationValue.Value.ToDouble() > 0)
                        {
                            writer.WriteAttributeString("duration", observationValue.Value);
                        }
                        break;

                    // ResetTriggered
                    case ValueKeys.ResetTriggered:
                        if (observationValue.Value != ResetTriggeredNotSpecified)
                        {
                            writer.WriteAttributeString("resetTriggered", observationValue.Value);
                        }
                        break;

                    // SampleRate
                    case ValueKeys.SampleRate:
                        if (observationValue.Value.ToDouble() > 0)
                        {
                            writer.WriteAttributeString("sampleRate", observationValue.Value);
                        }
                        break;

                    // Statistic
                    case ValueKeys.Statistic:
                        if (observationValue.Value != ResetTriggeredNotSpecified)
                        {
                            writer.WriteAttributeString("statistic", observationValue.Value);
                        }
                        break;

                    // Level (Ignore)
                    case ValueKeys.Level: break;

                    // Result (Ignore)
                    case ValueKeys.Result: break;

                    // Message (Ignore)
                    case ValueKeys.Message: break;

                    default:

                        // Check for namespace (skip for now, may be able to implement this better at some point)
                        if (observationValue.Key.StartsWith("Xmlns")) break;
                        if (observationValue.Key.StartsWith("xmlns")) break;

                        if (!string.IsNullOrEmpty(observationValue.Value))
                        {
                            writer.WriteAttributeString(ValueKeys.GetCamelCaseKey(observationValue.Key), observationValue.Value);
                        }

                        break;
                }
            }
        }

        private static void WriteDataSetXml(XmlWriter writer, IObservation observation)
        {
            if (observation.Values != null && observation.Values.Count() > 0)
            {
                var values = observation.Values.ToList();

                for (var i = 0; i < values.Count; i++)
                {
                    // Get DataSet Key
                    var key = ValueKeys.GetDataSetKey(values[i].Key);
                    if (!string.IsNullOrEmpty(key))
                    {
                        writer.WriteStartElement("Entry");
                        writer.WriteAttributeString("key", key);

                        if (values[i].Value != DataSetObservation.EntryRemovedValue)
                        {
                            writer.WriteString(values[i].Value);
                        }
                        else
                        {
                            writer.WriteAttributeString("removed", "true");
                        }

                        writer.WriteEndElement();
                    }
                }
            }
        }

        private static void WriteTableXml(XmlWriter writer, IObservation observation)
        {
            if (observation.Values != null && observation.Values.Count() > 0)
            {
                string previousEntryKey = null;
                string cellKey;
                bool firstEntryKey;
                bool found = false;
                bool first = true;

                var values = observation.Values.ToList();

                for (var i = 0; i < values.Count; i++)
                {
                    // Get Entry Key
                    var entrykey = ValueKeys.GetTableKey(values[i].Key);
                    if (!string.IsNullOrEmpty(entrykey))
                    {
                        found = true;

                        firstEntryKey = previousEntryKey != entrykey;
                        if (firstEntryKey)
                        {
                            // Close previous Entry
                            if (!first) writer.WriteEndElement();

                            // Create new Entry Element
                            writer.WriteStartElement("Entry");
                            writer.WriteAttributeString("key", entrykey);
                        }

                        // Get the CellKey
                        cellKey = ValueKeys.GetTableCellKey(values[i].Key);

                        // Create new Cell Element
                        writer.WriteStartElement("Cell");
                        writer.WriteAttributeString("key", cellKey);
                        writer.WriteString(values[i].Value);
                        writer.WriteEndElement();

                        previousEntryKey = entrykey;
                        first = false;
                    }
                }

                // Close previous Entry
                if (found) writer.WriteEndElement();
            }
        }

        private static void WriteTimeSeriesXml(XmlWriter writer, IObservation observation)
        {
            if (observation.Values != null && observation.Values.Count() > 0)
            {
                var found = false;
                var samples = new List<string>();

                var values = observation.Values.ToList();

                for (var i = 0; i < values.Count; i++)
                {
                    // Detect Timeseries Key
                    if (ValueKeys.IsTimeSeriesKey(values[i].Key))
                    {
                        found = true;

                        // Add sample to return list
                        samples.Add(values[i].Value);
                    }
                    else
                    {
                        if (found) break;
                    }
                }

                if (!samples.IsNullOrEmpty())
                {
                    // Write space delimited samples
                    writer.WriteString(string.Join(" ", samples));
                }
            }
        }
    }
}
