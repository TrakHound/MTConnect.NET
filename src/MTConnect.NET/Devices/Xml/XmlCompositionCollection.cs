// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlCompositionCollection : IXmlSerializable
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComposition));


        private List<Composition>_compositions;
        [XmlIgnore]
        public List<Composition> Compositions
        {
            get
            {
                if (_compositions == null) _compositions = new List<Composition>();
                return _compositions;
            }
            set
            {
                _compositions = value;
            }
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Compositions.IsNullOrEmpty())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                foreach (var composition in Compositions)
                {
                    try
                    {
                        // Create a new base class Composition to prevent the Type being required to be specified
                        // at compile time. This makes it possible to write custom Types
                        var obj = new XmlComposition();
                        obj.Id = composition.Id;
                        obj.Uuid = composition.Uuid;
                        obj.Name = composition.Name;
                        obj.NativeName = composition.NativeName;
                        obj.Type = composition.Type;
                        obj.Description = composition.Description;
                        obj.SampleRate = composition.SampleRate;
                        obj.SampleInterval = composition.SampleInterval;
                        //obj.References = composition.References;
                        if (composition.Configuration != null) obj.Configuration = new XmlConfiguration(composition.Configuration);

                        // DataItems
                        if (!composition.DataItems.IsNullOrEmpty())
                        {
                            obj.DataItemCollection = new XmlDataItemCollection { DataItems = composition.DataItems };
                        }

                        // Serialize the base class to a string
                        using (var dummyWriter = new StringWriter())
                        {
                            _serializer.Serialize(dummyWriter, obj, ns);
                            var xml = dummyWriter.ToString();

                            // Remove <?xml line
                            var startTag = "<Composition";
                            xml = xml.Substring(xml.IndexOf(startTag));

                            writer.WriteRaw(xml);
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
                            var doc = new System.Xml.XmlDocument();
                            var node = doc.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    // Create a new Node with the name of "Composition"
                                    var copy = doc.CreateNode(XmlNodeType.Element, "Composition", null);

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
                                    var composition = (XmlComposition)_serializer.Deserialize(new XmlNodeReader(copy));

                                    // Create new Composition based on Type (this gets this derived class instead of just the Composition base class)
                                    var obj = Composition.Create(child.Name);
                                    if (obj != null)
                                    {
                                        obj.Id = composition.Id;
                                        obj.Uuid = composition.Uuid;
                                        obj.Name = composition.Name;
                                        obj.NativeName = composition.NativeName;
                                        obj.Description = composition.Description;
                                        obj.SampleRate = composition.SampleRate;
                                        obj.SampleInterval = composition.SampleInterval;
                                        //obj.References = composition.References;
                                        if (composition.Configuration != null) obj.Configuration = composition.Configuration.ToConfiguration();

                                        // DataItems
                                        if (composition.DataItemCollection != null && !composition.DataItemCollection.DataItems.IsNullOrEmpty())
                                        {
                                            obj.DataItems = composition.DataItemCollection.DataItems;
                                        }

                                        Compositions.Add(obj);
                                    }
                                    else
                                    {
                                        // If no derived class is found then just add as base Composition
                                        composition.Type = child.Name;
                                        Compositions.Add(composition.ToComposition());
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
