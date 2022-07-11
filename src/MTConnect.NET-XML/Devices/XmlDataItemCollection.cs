// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    public class XmlDataItemCollection : IXmlSerializable
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDataItem));


        private List<DataItem> _dataItems;
        [XmlIgnore]
        public List<DataItem> DataItems
        {
            get
            {
                if (_dataItems == null) _dataItems = new List<DataItem>();
                return _dataItems;
            }
            set
            {
                _dataItems = value;
            }
        }


        public XmlDataItemCollection() { }

        public XmlDataItemCollection(IEnumerable<IDataItem> dataItems) 
        {
            if (!dataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in dataItems)
                {
                    DataItems.Add(new DataItem(dataItem));
                }
            }   
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!DataItems.IsNullOrEmpty())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                foreach (var dataItem in DataItems)
                {
                    if (!string.IsNullOrEmpty(dataItem.Type))
                    {
                        try
                        {
                            // Create a new base class DataItem to prevent the Type being required to be specified
                            // at compile time. This makes it possible to write custom Types
                            var obj = new XmlDataItem();
                            obj.DataItemCategory = dataItem.Category;
                            obj.Id = dataItem.Id;
                            obj.Name = dataItem.Name;
                            obj.Type = dataItem.Type;
                            obj.SubType = dataItem.SubType;
                            obj.NativeUnits = dataItem.NativeUnits;
                            obj.NativeScale = dataItem.NativeScale;
                            obj.SampleRate = dataItem.SampleRate;
                            obj.Source = dataItem.Source;
                            obj.Representation = dataItem.Representation;
                            obj.ResetTrigger = dataItem.ResetTrigger;
                            obj.CoordinateSystem = dataItem.CoordinateSystem;
                            obj.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
                            obj.Constraints = dataItem.Constraints;
                            obj.CompositionId = dataItem.CompositionId;
                            obj.Definition = dataItem.Definition;
                            obj.Units = dataItem.Units;
                            obj.Statistic = dataItem.Statistic;
                            obj.SignificantDigits = dataItem.SignificantDigits;
                            obj.Filters = dataItem.Filters;
                            obj.InitialValue = dataItem.InitialValue;

                            if (!dataItem.Relationships.IsNullOrEmpty())
                            {
                                var relationships = new List<XmlRelationship>();
                                foreach (var relationship in dataItem.Relationships)
                                {
                                    if (relationship.GetType() == typeof(DataItemRelationship))
                                    {
                                        relationships.Add(new XmlDataItemRelationship((DataItemRelationship)relationship));
                                    }

                                    if (relationship.GetType() == typeof(SpecificationRelationship))
                                    {
                                        relationships.Add(new XmlSpecificationRelationship((SpecificationRelationship)relationship));
                                    }
                                }
                                obj.Relationships = relationships;
                            }


                            // Serialize the base class to a string
                            var w = new StringWriter();
                            _serializer.Serialize(w, obj, ns);
                            var xml = w.ToString();

                            // Remove <?xml line
                            xml = xml.Substring(xml.IndexOf("<DataItem"));

                            // Write DataItem Type Description as Comment
                            writer.WriteComment($"Type = {dataItem.Type} : {dataItem.TypeDescription}");
                            writer.WriteWhitespace("\r\n");

                            // Write DataItem SubType Description as Comment
                            if (!string.IsNullOrEmpty(dataItem.SubType))
                            {
                                writer.WriteComment($"SubType = {dataItem.SubType} : {dataItem.SubTypeDescription}");
                                writer.WriteWhitespace("\r\n");
                            }

                            writer.WriteRaw(xml);
                        }
                        catch { }
                    }
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
                            var doc = new XmlDocument();
                            var node = doc.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    // Create a new Node with the name of "DataItem"
                                    var copy = doc.CreateNode(XmlNodeType.Element, "DataItem", null);

                                    // Copy Attributes
                                    foreach (XmlAttribute attribute in child.Attributes)
                                    {
                                        var attr = doc.CreateAttribute(attribute.Name);
                                        attr.Value = attribute.Value;
                                        copy.Attributes.Append(attr);
                                    }

                                    // Copy Text
                                    copy.InnerText = child.InnerText;
                                    copy.InnerXml = child.InnerXml;

                                    // Deserialize the copied Node to the Component base class
                                    var dataItem = (XmlDataItem)_serializer.Deserialize(new XmlNodeReader(copy));

                                    // Create new DataItem based on Type (this gets this derived class instead of just the DataItem base class)
                                    var obj = DataItem.Create(dataItem.Type);
                                    if (obj != null)
                                    {
                                        obj.Category = dataItem.DataItemCategory;
                                        obj.Id = dataItem.Id;
                                        obj.Name = dataItem.Name;
                                        obj.Type = dataItem.Type;
                                        obj.SubType = dataItem.SubType;
                                        obj.NativeUnits = dataItem.NativeUnits;
                                        obj.NativeScale = dataItem.NativeScale;
                                        obj.SampleRate = dataItem.SampleRate;
                                        obj.Source = dataItem.Source;
                                        obj.CompositionId = dataItem.CompositionId;
                                        obj.Representation = dataItem.Representation;
                                        obj.ResetTrigger = dataItem.ResetTrigger;
                                        obj.CoordinateSystem = dataItem.CoordinateSystem;
                                        obj.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
                                        obj.Constraints = dataItem.Constraints;
                                        obj.Definition = dataItem.Definition;
                                        obj.Units = dataItem.Units;
                                        obj.Statistic = dataItem.Statistic;
                                        obj.SignificantDigits = dataItem.SignificantDigits;
                                        obj.Filters = dataItem.Filters;
                                        obj.InitialValue = dataItem.InitialValue;

                                        if (!dataItem.Relationships.IsNullOrEmpty())
                                        {
                                            var relationships = new List<Relationship>();
                                            foreach (var relationship in dataItem.Relationships)
                                            {
                                                relationships.Add(relationship.ToRelationship());
                                            }
                                            obj.Relationships = relationships;
                                        }

                                        DataItems.Add(obj);
                                    }
                                    else
                                    {
                                        // If no derived class is found then just add as base DataItem
                                        DataItems.Add(dataItem.ToDataItem());
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
