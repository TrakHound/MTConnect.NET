// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Custom <see cref="IXmlSerializable"/> for the child components of a
    /// component or device. Because MTConnect encodes a component's type in its
    /// element name, this collection serializes each component through the
    /// shared base surrogate and rewrites the element name to the concrete
    /// component type, and reverses that on read.
    /// </summary>
    public class XmlComponentCollection : IXmlSerializable
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComponent));
        private readonly bool _outputComments = false;


        private List<XmlComponent> _components;

        /// <summary>
        /// The child components held by the collection; never <c>null</c>.
        /// </summary>
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


        /// <summary>
        /// Creates an empty collection.
        /// </summary>
        public XmlComponentCollection() { }

        /// <summary>
        /// Creates an empty collection, recording whether component type
        /// descriptions should be emitted as comments.
        /// </summary>
        public XmlComponentCollection(bool outputComments = false)
        {
            _outputComments = outputComments;
        }


        #region "Xml Serialization"

        /// <summary>
        /// Writes each component to <paramref name="writer"/>, serializing
        /// through the base surrogate and renaming the element to the concrete
        /// component type, optionally preceded by a type-description comment.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            if (!Components.IsNullOrEmpty())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                foreach (var component in Components)
                {
                    if (!string.IsNullOrEmpty(component.Type))
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

                                if (xml.EndsWith("/>"))
                                {
                                    // Replace the base class Component start tag name with the derived Component Type
                                    xml = xml.Substring(startTag.Length + 1);
                                    xml = $"<{component.Type} " + xml;
                                }
                                else
                                {
                                    // Replace the base class Component start tag name with the derived Component Type
                                    xml = xml.Substring(startTag.Length + 1);
                                    xml = $"<{component.Type} " + xml;

                                    // Replace the base class Component End tag with the derived Component Type
                                    var endTag = "</Component>";
                                    xml = xml.Substring(0, xml.Length - endTag.Length);
                                    xml = xml + $"</{component.Type}>";
                                }

                                if (_outputComments)
                                {
                                    // Write Component Type Description as Comment
                                    writer.WriteComment($"Type = {component.Type} : {component.TypeDescription}");
                                    writer.WriteWhitespace("\r\n");
                                }

                                writer.WriteRaw(xml);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Reads the component child elements from <paramref name="reader"/>,
        /// rewriting each type-named element to the base <c>Component</c> name
        /// so it can be deserialized, then restoring the concrete type.
        /// </summary>
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

        /// <summary>
        /// Returns <c>null</c>; the collection does not advertise an inline XML
        /// schema, as required by <see cref="IXmlSerializable"/>.
        /// </summary>
        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion
    }
}