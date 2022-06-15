// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace MTConnect.Streams
{
    internal static class XmlObservation
    {
        public const string DataSetSuffix = "DataSet";
        public const string TableSuffix = "Table";
        public const string TimeSeriesSuffix = "TimeSeries";
        public const string XmlAttributeName = "XmlAttributeAttribute";


        #region "Representations"

        public static void SetValue(Observation observation, XmlNode node)
        {
            if (observation != null && node != null)
            {
                observation.AddValue(new ObservationValue(ValueKeys.CDATA, node.InnerText));
            }
        }

        public static void SetDataSetEntries(Observation observation, XmlNode node)
        {
            if (node != null && node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        if (childNode.Name == "Entry")
                        {
                            // Get "key" attribute of Entry node
                            var key = childNode.Attributes["key"];
                            if (key != null)
                            {
                                // Get value from CDATA of Entry node
                                var value = childNode.InnerText;

                                observation.AddValue(ValueKeys.CreateDataSetValueKey(key.Value), value);
                            }
                        }
                    }
                }
            }
        }

        public static void SetTableEntries(Observation observation, XmlNode node)
        {
            if (node != null && node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        if (childNode.Name == "Entry")
                        {
                            // Get "key" attribute of Entry node
                            var entryKey = childNode.Attributes["key"];
                            if (entryKey != null)
                            {
                                if (childNode.HasChildNodes)
                                {
                                    foreach (XmlNode subChildNode in childNode.ChildNodes)
                                    {
                                        if (subChildNode.NodeType == XmlNodeType.Element)
                                        {
                                            if (subChildNode.Name == "Cell")
                                            {
                                                // Get "key" attribute of Cell node
                                                var cellKey = subChildNode.Attributes["key"];

                                                // Get value from CDATA of Cell node
                                                var value = subChildNode.InnerText;

                                                observation.AddValue(ValueKeys.CreateTableValueKey(entryKey.Value, cellKey.Value), value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SetTimeSeriesEntries(Observation observation, XmlNode node)
        {
            if (node != null)
            {
                if (!string.IsNullOrEmpty(node.InnerText))
                {
                    var samples = node.InnerText.Split(' ');
                    if (!samples.IsNullOrEmpty())
                    {
                        for (var i = 0; i < samples.Count() - 1; i++)
                        {
                            observation.AddValue(ValueKeys.CreateTimeSeriesValueKey(i), samples[i]);
                        }
                    }
                }
            }
        }

        #endregion

        #region "Properties & Attribute Mapping"

        public struct ObservationProperty
        {
            public string Name { get; set; }

            public string Value { get; set; }

            public ObservationProperty(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public static void AddAttributes(Observation observation, XmlNode node)
        {
            if (node != null && node.OwnerDocument != null)
            {
                var properties = GetProperties(observation);
                if (!properties.IsNullOrEmpty())
                {
                    foreach (var property in properties)
                    {
                        // Create an XmlAttribute for the Member
                        XmlAttribute attribute = node.OwnerDocument.CreateAttribute(property.Name);
                        attribute.Value = property.Value;
                        node.Attributes.Append(attribute);
                    }
                }
            }
        }

        public static string ReadAttributeValue(XmlNode node, string attributeName)
        {
            if (node != null && attributeName != null)
            {
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (attribute.Name == attributeName)
                        {
                            return attribute.Value;
                        }
                    }
                }
            }

            return null;
        }

        public static void ReadAttributes(Observation observation, XmlNode node)
        {
            if (observation != null && node != null && node.OwnerDocument != null)
            {
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        object attributeValue = attribute.Value;

                        var property = GetProperty(observation, attribute);
                        if (attributeValue != null && property != null)
                        {
                            object propertyValue = null;

                            if (property.PropertyType == typeof(DateTime))
                            {
                                if (DateTime.TryParse(attributeValue.ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal, out var dateTime)) propertyValue = dateTime.ToUniversalTime();
                            }
                            else
                            {
                                propertyValue = attributeValue;
                            }

                            // Set the Property for the Observation
                            observation.SetProperty(property.Name, propertyValue);
                        }
                    }
                }
            }
        }

        public static IEnumerable<ObservationProperty> GetProperties(object observation)
        {
            var objs = new List<ObservationProperty>();

            if (observation != null)
            {
                // Get Observation Type
                var dataItemType = observation.GetType();

                // Get Type Properties (includes derived Type properties)
                var properties = dataItemType.GetProperties();
                if (!properties.IsNullOrEmpty())
                {
                    foreach (var property in properties.OrderBy(o => o.Name))
                    {
                        // Check for {PropertyName}Output Property to determine whether to output or not
                        var output = GetPropertyOutputFlag(dataItemType, property.Name, observation);
                        if (output)
                        {
                            // Get the XmlAttribute Attribute
                            var attribute = GetAttribute(property);
                            if (attribute != null)
                            {
                                // Get the Property Value
                                var value = property.GetValue(observation);

                                // Get Default Value for the Property Type
                                var defaultValue = GetDefaultValue(property.PropertyType);

                                if (value != defaultValue && value != null)
                                {
                                    string valueString = null;

                                    if (property.PropertyType == typeof(DateTime))
                                    {
                                        valueString = ((DateTime)value).ToString("o");
                                    }
                                    else
                                    {
                                        valueString = value.ToString();
                                    }

                                    objs.Add(new ObservationProperty(attribute, valueString));
                                }
                            }
                        }
                    }
                }
            }

            return objs;
        }

        private static PropertyInfo GetProperty(object obj, XmlAttribute attribute)
        {
            if (obj != null && attribute != null)
            {
                var objType = obj.GetType();

                var properties = objType.GetProperties();
                if (!properties.IsNullOrEmpty())
                {
                    foreach (var property in properties)
                    {
                        if (attribute.Name == property.Name.LowercaseFirstCharacter())
                        {
                            return property;
                        }
                    }
                }
            }

            return null;
        }

        private static string GetAttribute(PropertyInfo property)
        {
            if (property != null)
            {
                return property.Name.LowercaseFirstCharacter();
            }

            return null;
        }

        private static bool GetPropertyOutputFlag(Type type, string propertyName, object obj)
        {
            if (type != null && !string.IsNullOrEmpty(propertyName) && obj != null)
            {
                var properties = type.GetRuntimeProperties();
                if (properties != null)
                {
                    var outputProperty = properties.FirstOrDefault(o => o.Name == $"{propertyName}Output");
                    if (outputProperty != null)
                    {
                        return (bool)outputProperty.GetValue(obj);
                    }
                }
            }

            return true;
        }


        private static object GetDefaultValue(Type t)
        {
            try
            {
                if (t.IsValueType) return Activator.CreateInstance(t);
            }
            catch { }

            return null;
        }

        #endregion


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

        public static DataItemRepresentation GetDataItemRepresentation(string elementName)
        {
            if (IsDataSet(elementName)) return DataItemRepresentation.DATA_SET;
            if (IsTable(elementName)) return DataItemRepresentation.TABLE;
            if (IsTimeSeries(elementName)) return DataItemRepresentation.TIME_SERIES;
            else return DataItemRepresentation.VALUE;
        }

        public static string GetDataItemType(string elementName)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                if (IsDataSet(elementName) && elementName.Length > DataSetSuffix.Length)
                {
                    // Remove the "DataSet" suffix from the Type
                    return elementName.Substring(0, elementName.Length - DataSetSuffix.Length);
                }
                else if (IsTable(elementName) && elementName.Length > TableSuffix.Length)
                {
                    // Remove the "Table" suffix from the Type
                    return elementName.Substring(0, elementName.Length - TableSuffix.Length);
                }
                else if (IsTimeSeries(elementName) && elementName.Length > TimeSeriesSuffix.Length)
                {
                    // Remove the "TimeSeries" suffix from the Type
                    return elementName.Substring(0, elementName.Length - TimeSeriesSuffix.Length);
                }
                else
                {
                    return elementName;
                }
            }

            return null;
        }
    }
}
