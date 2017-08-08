// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.MTConnectStreams
{
    public class DataItemCollection<T> : IXmlSerializable
    {
        private static XmlSerializer conditionSerializer = new XmlSerializer(typeof(Condition));
        private static XmlSerializer eventSerializer = new XmlSerializer(typeof(Event));
        private static XmlSerializer sampleSerializer = new XmlSerializer(typeof(Condition));


        [XmlIgnore]
        public List<DataItem> DataItems { get; set; }


        public DataItemCollection()
        {
            DataItems = new List<DataItem>();

            if (typeof(T) == typeof(Condition)) type = DataItemType.Condition;
            else if (typeof(T) == typeof(Event)) type = DataItemType.Event;
            else type = DataItemType.Sample;

            // Create a dummy XmlDocument to use create dummy nodes
            doc = new XmlDocument();

            // Create a new Node with the name of "DataItem"
            copy = doc.CreateNode(XmlNodeType.Element, type.ToString(), null);
        }

        private enum DataItemType
        {
            Condition,
            Event,
            Sample
        }

        private DataItemType type;
        private XmlDocument doc;
        private XmlNode copy;


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString("");
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Elements
                using (var inner = reader.ReadSubtree())
                {

                    XmlSerializer serializer;
                    switch (type)
                    {
                        case DataItemType.Condition: serializer = new XmlSerializer(typeof(Condition)); break;
                        case DataItemType.Event: serializer = new XmlSerializer(typeof(Event)); break;
                        default: serializer = new XmlSerializer(typeof(Sample)); break;
                    }

                    while (inner.Read())
                    {
                        if (inner.NodeType == XmlNodeType.Element)
                        {
                            // Create a copy of each Child Node so we can change the name to "DataItem" and deserialize it
                            // (Seems like a dirty way to do this but until an XmlAttribute can be found to ignore the Node's name/type
                            // and to always deserialize as a DataItem)
                            // Condition uses the node name as the condition value
                            var node = doc.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    // Copy Attributes
                                    foreach (XmlAttribute attribute in child.Attributes)
                                    {
                                        var attr = doc.CreateAttribute(attribute.Name);
                                        attr.Value = attribute.Value;
                                        copy.Attributes.Append(attr);
                                    }

                                    // Copy Text
                                    copy.InnerText = child.InnerText;

                                    // Deserialize as DataItem
                                    switch (type)
                                    {
                                        case DataItemType.Condition:

                                            var _condition = (Condition)serializer.Deserialize(new XmlNodeReader(copy));
                                            ConditionValue value;
                                            Enum.TryParse(child.Name.ToUpper(), out value);
                                            _condition.ConditionValue = value;
                                            _condition.Type = child.Name;
                                            DataItems.Add(_condition);
                                            break;

                                        case DataItemType.Event:

                                            var _event = (Event)serializer.Deserialize(new XmlNodeReader(copy));
                                            _event.Type = child.Name;
                                            DataItems.Add(_event);
                                            break;

                                        default:

                                            var _sample = (Sample)serializer.Deserialize(new XmlNodeReader(copy));
                                            _sample.Type = child.Name;
                                            DataItems.Add(_sample);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
