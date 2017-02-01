// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices
{
    public class ComponentCollection : IXmlSerializable
    {
        [XmlIgnore]
        public List<Component> Components { get; set; }


        public ComponentCollection() { Components = new List<Component>(); }

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
                    var serializer = new XmlSerializer(typeof(Component));

                    while (inner.Read())
                    {
                        if (inner.NodeType == XmlNodeType.Element)
                        {
                            // Create a copy of each Child Node so we can change the name to "Component" and deserialize it
                            // (Seems like a dirty way to do this but until an XmlAttribute can be found to ignore the Node's name/type
                            // and to always deserialize as a Component)
                            var doc = new XmlDocument();
                            var node = doc.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    // Create a new Node with the name of "Component"
                                    var copy = doc.CreateNode(XmlNodeType.Element, "Component", null);

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

                                    // Create new Component
                                    var component = (Component)serializer.Deserialize(new XmlNodeReader(copy));
                                    component.Type = child.Name;
                                    Components.Add(component);
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
