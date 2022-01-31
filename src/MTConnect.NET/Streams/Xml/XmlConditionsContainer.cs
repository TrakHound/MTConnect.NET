// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// Conditions is a XML container type element. 
    /// Conditions organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of CONDITION in the MTConnectDevices document.
    /// </summary>
    public class XmlConditionsContainer : IXmlSerializable
    {
        private const string NodePrefixPattern = "(.*):(.*)";

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Condition));

        private readonly XmlDocument _document;
        private readonly XmlNode _copy;

        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of CONDITION.
        /// </summary>
        [XmlIgnore]
        public List<Condition> Conditions { get; set; }


        public XmlConditionsContainer()
        {
            // Initialize the Conditions List
            Conditions = new List<Condition>();

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();

            // Create a new Node with the name of "DataItem"
            _copy = _document.CreateNode(XmlNodeType.Element, Devices.DataItemCategory.CONDITION.ToString().ToPascalCase(), null);
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Conditions.IsNullOrEmpty())
            {
                foreach (var dataItem in Conditions)
                {
                    try
                    {
                        var type = dataItem.Level.ToString();
                        if (!string.IsNullOrEmpty(type))
                        {
                            string prefix = null;
                            string name = null;

                            var match = Regex.Match(type, NodePrefixPattern);
                            if (match.Success)
                            {
                                prefix = match.Groups[1].Value;
                                name = match.Groups[2].Value.ToPascalCase();
                            }
                            else
                            {
                                name = type.ToPascalCase();
                            }

                            var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                            if (node != null)
                            {
                                node.InnerText = dataItem.CDATA?.Trim();

                                // DataItemId
                                if (!string.IsNullOrEmpty(dataItem.DataItemId))
                                {
                                    var aDataItemId = _document.CreateAttribute("dataItemId");
                                    aDataItemId.Value = dataItem.DataItemId;
                                    node.Attributes.Append(aDataItemId);
                                }

                                // Timestamp
                                var aTimestamp = _document.CreateAttribute("timestamp");
                                aTimestamp.Value = dataItem.Timestamp.ToString("o");
                                node.Attributes.Append(aTimestamp);

                                // Name
                                if (!string.IsNullOrEmpty(dataItem.Name))
                                {
                                    var aName = _document.CreateAttribute("name");
                                    aName.Value = dataItem.Name;
                                    node.Attributes.Append(aName);
                                }

                                // Sequence
                                var aSequence = _document.CreateAttribute("sequence");
                                aSequence.Value = dataItem.Sequence.ToString();
                                node.Attributes.Append(aSequence);

                                // Type
                                if (!string.IsNullOrEmpty(dataItem.Type))
                                {
                                    var aType = _document.CreateAttribute("type");
                                    aType.Value = dataItem.Type;
                                    node.Attributes.Append(aType);
                                }

                                // SubType
                                if (!string.IsNullOrEmpty(dataItem.SubType))
                                {
                                    var aSubType = _document.CreateAttribute("subType");
                                    aSubType.Value = dataItem.SubType;
                                    node.Attributes.Append(aSubType);
                                }

                                // Native Code
                                if (!string.IsNullOrEmpty(dataItem.NativeCode))
                                {
                                    var aNativeCode = _document.CreateAttribute("nativeCode");
                                    aNativeCode.Value = dataItem.NativeCode;
                                    node.Attributes.Append(aNativeCode);
                                }

                                // Native Severity
                                if (!string.IsNullOrEmpty(dataItem.NativeSeverity))
                                {
                                    var aNativeSeverity = _document.CreateAttribute("nativeSeverity");
                                    aNativeSeverity.Value = dataItem.NativeSeverity;
                                    node.Attributes.Append(aNativeSeverity);
                                }

                                // Qualifier
                                if (!string.IsNullOrEmpty(dataItem.Qualifier))
                                {
                                    var aQualifier = _document.CreateAttribute("qualifier");
                                    aQualifier.Value = dataItem.Qualifier;
                                    node.Attributes.Append(aQualifier);
                                }

                                // Statistic
                                if (!string.IsNullOrEmpty(dataItem.Statistic))
                                {
                                    var aStatistic = _document.CreateAttribute("statistic");
                                    aStatistic.Value = dataItem.Statistic;
                                    node.Attributes.Append(aStatistic);
                                }

                                writer.WriteNode(new XmlNodeReader(node), false);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Elements
                using (var inner = reader.ReadSubtree())
                {
                    while (inner.Read())
                    {
                        if (inner.NodeType == XmlNodeType.Element)
                        {
                            // Create a copy of each Child Node so we can change the name to "DataItem" and deserialize it
                            // (Seems like a dirty way to do this but until an XmlAttribute can be found to ignore the Node's name/type
                            // and to always deserialize as a DataItem)
                            // Condition uses the node name as the condition value
                            var node = _document.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    // Clear any previous Attributes
                                    _copy.Attributes.RemoveAll();

                                    // Copy Attributes
                                    foreach (XmlAttribute attribute in child.Attributes)
                                    {
                                        var attr = _document.CreateAttribute(attribute.Name);
                                        attr.Value = attribute.Value;
                                        _copy.Attributes.Append(attr);
                                    }

                                    // Copy Text
                                    _copy.InnerText = child.InnerText;
                                    _copy.InnerXml = child.InnerXml;

                                    var dataItem = (Condition)_serializer.Deserialize(new XmlNodeReader(_copy));
                                    if (Enum.TryParse<ConditionLevel>(child.Name.ToUpper(), out var level))
                                    {
                                        dataItem.Level = level;
                                        dataItem.Type = child.Name;
                                        Conditions.Add(dataItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // Advance Reader
            reader.Skip();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion
    }
}
