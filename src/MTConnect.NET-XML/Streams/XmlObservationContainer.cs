// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// Samples is a XML container type element. 
    /// Samples organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    internal class XmlObservationContainer
    {
        private const string ConditionUnavailable = "Unavailable";
        private const string ConditionFault = "Fault";
        private const string ConditionWarning = "Warning";
        private const string ConditionNormal = "Normal";

        private const string ResetTriggeredNotSpecified = "NOT_SPECIFIED";

        private bool _outputComments = false;


        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<IObservationOutput> Observations { get; set; }


        public XmlObservationContainer(bool outputComments = false, int capacity = 0)
        {
            _outputComments = outputComments;

            // Initialize the Observations List
            if (capacity > 0) Observations = new List<IObservationOutput>(capacity);
            else Observations = new List<IObservationOutput>();
        }

        public XmlObservationContainer(IObservationOutput observation, bool outputComments = false, int capacity = 0)
        {
            _outputComments = outputComments;

            if (capacity > 0) Observations = new List<IObservationOutput>(capacity);
            else Observations = new List<IObservationOutput>();

            if (observation != null) Observations.Add(observation);
        }

        public XmlObservationContainer(IEnumerable<IObservationOutput> observations, bool outputComments = false)
        {
            _outputComments = outputComments;
            
            if (!observations.IsNullOrEmpty())
            {
                Observations = new List<IObservationOutput>(observations.Count());
                Observations.AddRange(observations);
            }
            else
            {
                Observations = new List<IObservationOutput>();
            }
        }


        public static void WriteXml(
            XmlWriter writer,
            ref IObservationOutput[] observations,
            DataItemCategory category,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            bool outputComments = false
            )
        {
            if (observations != null && observations.Length > 0)
            {
                var first = true;
                var found = false;

                for (var i = 0; i < observations.Length; i++)
                {
                    var observation = observations[i];
                    if (observation != null && observation.Category == category)
                    {
                        if (first)
                        {
                            found = true;
                            first = false;

                            // Write Start Element for Container
                            switch (category)
                            {
                                case DataItemCategory.SAMPLE: writer.WriteStartElement("Samples"); break;
                                case DataItemCategory.EVENT: writer.WriteStartElement("Events"); break;
                                case DataItemCategory.CONDITION: writer.WriteStartElement("Condition"); break;
                            }
                        }

                        // Write Observation
                        WriteXml(writer, ref observation, extendedSchemas, outputComments);
                    }
                }

                // Write Closing Element
                if (found) writer.WriteEndElement();
            }
        }

        private static void WriteXml(
            XmlWriter writer,
            ref IObservationOutput observation,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            bool outputComments = false
            )
        {
            try
            {
                // Get Element name
                var name = GetElementName(ref observation);
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
                        // Write Common Values
                        for (var i = 0; i < observation.Values.Length; i++)
                        {
                            WriteCommonValuesXml(writer, ref observation, ref observation.Values[i]);
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
                            case DataItemRepresentation.DATA_SET: WriteDataSetXml(writer, ref observation); break;
                            case DataItemRepresentation.TABLE: WriteTableXml(writer, ref observation); break;
                            case DataItemRepresentation.TIME_SERIES: WriteTimeSeriesXml(writer, ref observation); break;
                        }
                    }

                    writer.WriteEndElement();
                }
            }
            catch { }
        }


        private static string GetElementName(ref IObservationOutput observation)
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


        private static void WriteCommonValuesXml(XmlWriter writer, ref IObservationOutput observation, ref ObservationValue observationValue)
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

        private static void WriteDataSetXml(XmlWriter writer, ref IObservationOutput observation)
        {
            if (observation.Values != null && observation.Values.Length > 0)
            {
                for (var i = 0; i < observation.Values.Length; i++)
                {
                    // Get DataSet Key
                    var key = ValueKeys.GetDataSetKey(observation.Values[i].Key);
                    if (!string.IsNullOrEmpty(key))
                    {
                        writer.WriteStartElement("Entry");
                        writer.WriteAttributeString("key", key);

                        if (observation.Values[i].Value != DataSetObservation.EntryRemovedValue)
                        {
                            writer.WriteString(observation.Values[i].Value);
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

        private static void WriteTableXml(XmlWriter writer, ref IObservationOutput observation)
        {
            if (observation.Values != null && observation.Values.Length > 0)
            {
                string previousEntryKey = null;
                string cellKey;
                bool firstEntryKey;
                bool found = false;
                bool first = true;

                for (var i = 0; i < observation.Values.Length; i++)
                {
                    // Get Entry Key
                    var entrykey = ValueKeys.GetTableKey(observation.Values[i].Key);
                    if (!string.IsNullOrEmpty(entrykey))
                    {
                        found = true;
                        var removed = observation.Values[i].Value == TableObservation.EntryRemovedValue;

                        firstEntryKey = previousEntryKey != entrykey;
                        if (firstEntryKey)
                        {
                            // Close previous Entry
                            if (!first) writer.WriteEndElement();

                            // Create new Entry Element
                            writer.WriteStartElement("Entry");
                            writer.WriteAttributeString("key", entrykey);

                            if (removed)
                            {
                                writer.WriteAttributeString("removed", "true");
                            }
                        }

                        if (!removed)
                        {
                            // Get the CellKey
                            cellKey = ValueKeys.GetTableCellKey(observation.Values[i].Key);

                            // Create new Cell Element
                            writer.WriteStartElement("Cell");
                            writer.WriteAttributeString("key", cellKey);
                            writer.WriteString(observation.Values[i].Value);
                            writer.WriteEndElement();
                        }

                        previousEntryKey = entrykey;
                        first = false;
                    }
                }

                // Close previous Entry
                if (found) writer.WriteEndElement();
            }
        }

        private static void WriteTimeSeriesXml(XmlWriter writer, ref IObservationOutput observation)
        {
            if (observation.Values != null && observation.Values.Length > 0)
            {
                var found = false;
                var samples = new List<string>();

                for (var i = 0; i < observation.Values.Length; i++)
                {
                    // Detect Timeseries Key
                    if (ValueKeys.IsTimeSeriesKey(observation.Values[i].Key))
                    {
                        found = true;

                        // Add sample to return list
                        samples.Add(observation.Values[i].Value);
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