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
    public class XmlComponentCollection : IXmlSerializable
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComponent));


        private List<XmlComponent> _components;
        [XmlIgnore]
        public List<XmlComponent> Components
        {
            get
            {
                if (_components == null) _components = new List<XmlComponent>();
                return _components;
            }
            set
            {
                _components = value;
            }
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Components.IsNullOrEmpty())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                foreach (var component in Components)
                {
                    try
                    {
                        // Serialize the base class to a string
                        using (var dummyWriter = new StringWriter())
                        {
                            _serializer.Serialize(dummyWriter, component, ns);
                            var xml = dummyWriter.ToString();

                            // Remove <?xml line
                            var startTag = "<Component";
                            xml = xml.Substring(xml.IndexOf(startTag));

                            // Replace the base class Component start tag name with the derived Component Type
                            xml = xml.Substring(startTag.Length + 1);
                            xml = $"<{component.Type} " + xml;

                            // Replace the base class Component End tag with the derived Component Type
                            var endTag = "</Component>";
                            xml = xml.Substring(0, xml.Length - endTag.Length);
                            xml = xml + $"</{component.Type}>";

                            writer.WriteRaw(xml);
                        }
                    }
                    catch { }
                }
            }
        }

        //public void WriteXml(XmlWriter writer)
        //{
        //    if (!Components.IsNullOrEmpty())
        //    {
        //        var ns = new XmlSerializerNamespaces();
        //        ns.Add("", "");

        //        foreach (var component in Components)
        //        {
        //            try
        //            {
        //                // Create a new base class Component to prevent the Type being required to be specified
        //                // at compile time. This makes it possible to write custom Types
        //                var obj = new XmlComponent();
        //                obj.Id = component.Id;
        //                obj.Uuid = component.Uuid;
        //                obj.Name = component.Name;
        //                obj.NativeName = component.NativeName;
        //                obj.Type = component.Type;
        //                obj.Description = component.Description;
        //                obj.SampleRate = component.SampleRate;
        //                obj.SampleInterval = component.SampleInterval;
        //                obj.References = component.References;
        //                if (component.Configuration != null) obj.Configuration = new XmlConfiguration(component.Configuration);

        //                // DataItems
        //                if (!component.DataItems.IsNullOrEmpty())
        //                {
        //                    foreach (var dataItem in component.DataItems)
        //                    {
        //                        obj.DataItems.Add(new XmlDataItem(dataItem));
        //                    }
        //                }

        //                // Compositions
        //                if (!component.Compositions.IsNullOrEmpty())
        //                {
        //                    foreach (var composition in component.Compositions)
        //                    {
        //                        obj.Compositions.Add(new XmlComposition(composition));
        //                    }
        //                }

        //                // Components
        //                if (!component.Components.IsNullOrEmpty())
        //                {
        //                    obj.ComponentCollection = new XmlComponentCollection { Components = component.Components };
        //                }

        //                // Serialize the base class to a string
        //                using (var dummyWriter = new StringWriter())
        //                {
        //                    _serializer.Serialize(dummyWriter, obj, ns);
        //                    var xml = dummyWriter.ToString();

        //                    // Remove <?xml line
        //                    var startTag = "<Component";
        //                    xml = xml.Substring(xml.IndexOf(startTag));

        //                    // Replace the base class Component start tag name with the derived Component Type
        //                    xml = xml.Substring(startTag.Length + 1);
        //                    xml = $"<{component.Type} " + xml;

        //                    // Replace the base class Component End tag with the derived Component Type
        //                    var endTag = "</Component>";
        //                    xml = xml.Substring(0, xml.Length - endTag.Length);
        //                    xml = xml + $"</{component.Type}>";

        //                    writer.WriteRaw(xml);
        //                }                  
        //            }
        //            catch { }
        //        }
        //    }
        //}

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

                                    // Deserialize the copied Node to the Component base class
                                    var component = (XmlComponent)_serializer.Deserialize(new XmlNodeReader(copy));
                                    component.Type = child.Name;
                                    Components.Add(component);
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
