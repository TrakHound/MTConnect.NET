// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlComponentCollection : IXmlSerializable
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlComponent));


        private List<Component> _components;
        [XmlIgnore]
        public List<Component> Components
        {
            get
            {
                if (_components == null) _components = new List<Component>();
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
                        // Create a new base class Component to prevent the Type being required to be specified
                        // at compile time. This makes it possible to write custom Types
                        var obj = new XmlComponent();
                        obj.Id = component.Id;
                        obj.Uuid = component.Uuid;
                        obj.Name = component.Name;
                        obj.NativeName = component.NativeName;
                        obj.Type = component.Type;
                        obj.Description = component.Description;
                        obj.SampleRate = component.SampleRate;
                        obj.SampleInterval = component.SampleInterval;
                        obj.References = component.References;
                        obj.Configuration = component.Configuration;

                        // DataItems
                        if (!component.DataItems.IsNullOrEmpty())
                        {
                            foreach (var dataItem in component.DataItems)
                            {
                                obj.DataItems.Add(new XmlDataItem(dataItem));
                            }
                        }

                        // Compositions
                        if (!component.Compositions.IsNullOrEmpty())
                        {
                            foreach (var composition in component.Compositions)
                            {
                                obj.Compositions.Add(new XmlComposition(composition));
                            }
                        }

                        //// DataItems
                        //if (!component.DataItems.IsNullOrEmpty())
                        //{
                        //    obj.DataItemCollection = new XmlDataItemCollection { DataItems = component.DataItems };
                        //}

                        //// Compositions
                        //if (!component.Compositions.IsNullOrEmpty())
                        //{
                        //    obj.CompositionCollection = new XmlCompositionCollection { Compositions = component.Compositions };
                        //}

                        // Components
                        if (!component.Components.IsNullOrEmpty())
                        {
                            obj.ComponentCollection = new XmlComponentCollection { Components = component.Components };
                        }

                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    var writerSettings = new XmlWriterSettings
                        //    {
                        //        Encoding = Encoding.UTF8,
                        //        Indent = true                            
                        //    };

                        //    using (var streamWriter = XmlWriter.Create(memoryStream, writerSettings))
                        //    {
                        //        // Serialize XML Document
                        //        _serializer.Serialize(streamWriter, obj, ns);

                        //        // Convert Byte Array to UTF8 string
                        //        var xml = Encoding.UTF8.GetString(memoryStream.ToArray());

                        //        // Remove <?xml line
                        //        var startTag = "<Component";
                        //        xml = xml.Substring(xml.IndexOf(startTag));

                        //        // Replace the base class Component start tag name with the derived Component Type
                        //        xml = xml.Substring(startTag.Length + 1);
                        //        xml = $"<{component.Type} " + xml;

                        //        // Replace the base class Component End tag with the derived Component Type
                        //        var endTag = "</Component>";
                        //        xml = xml.Substring(0, xml.Length - endTag.Length);
                        //        xml = xml + $"</{component.Type}>";

                        //        //xml = xml.Replace("<Component ", $"<{component.Type} ");

                        //        writer.WriteWhitespace("\n");
                        //        writer.WriteRaw(xml);
                        //    }
                        //}



                        // Serialize the base class to a string
                        using (var dummyWriter = new StringWriter())
                        {
                            _serializer.Serialize(dummyWriter, obj, ns);
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
        //                obj.Configuration = component.Configuration;

        //                // DataItems
        //                if (!component.DataItems.IsNullOrEmpty())
        //                {
        //                    foreach (var dataItem in component.DataItems)
        //                    {
        //                        obj.DataItems.Add(new XmlDataItem(dataItem));
        //                    }
        //                }

        //                //// DataItems
        //                //if (!component.DataItems.IsNullOrEmpty())
        //                //{
        //                //    obj.DataItemCollection = new XmlDataItemCollection { DataItems = component.DataItems };
        //                //}

        //                // Compositions
        //                if (!component.Compositions.IsNullOrEmpty())
        //                {
        //                    obj.CompositionCollection = new XmlCompositionCollection { Compositions = component.Compositions };
        //                }

        //                // Components
        //                if (!component.Components.IsNullOrEmpty())
        //                {
        //                    obj.ComponentCollection = new XmlComponentCollection { Components = component.Components };
        //                }



        //                // Serialize the base class to a string
        //                //using (var )
        //                var w = new StringWriter();
        //                _serializer.Serialize(w, obj, ns);
        //                var xml = w.ToString();

        //                // Remove <?xml line
        //                var startTag = "<Component";
        //                xml = xml.Substring(xml.IndexOf(startTag));

        //                // Replace the base class Component start tag name with the derived Component Type
        //                xml = xml.Substring(startTag.Length + 1);
        //                xml = $"<{component.Type} " + xml;

        //                // Replace the base class Component End tag with the derived Component Type
        //                var endTag = "</Component>";
        //                xml = xml.Substring(0, xml.Length - endTag.Length);
        //                xml = xml + $"</{component.Type}>";

        //                //xml = xml.Replace("<Component ", $"<{component.Type} ");

        //                writer.WriteRaw(xml);
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

                                    // Create new Component based on Type (this gets this derived class instead of just the Component base class)
                                    var obj = Component.Create(child.Name);
                                    if (obj != null)
                                    {
                                        obj.Id = component.Id;
                                        obj.Uuid = component.Uuid;
                                        obj.Name = component.Name;
                                        obj.NativeName = component.NativeName;
                                        obj.Description = component.Description;
                                        obj.SampleRate = component.SampleRate;
                                        obj.SampleInterval = component.SampleInterval;
                                        obj.References = component.References;
                                        obj.Configuration = component.Configuration;

                                        // DataItems
                                        if (!component.DataItems.IsNullOrEmpty())
                                        {
                                            obj.DataItems = new List<DataItem>();
                                            foreach (var dataItem in component.DataItems)
                                            {
                                                obj.DataItems.Add(dataItem.ToDataItem());
                                            }
                                        }

                                        // Compositions
                                        if (!component.Compositions.IsNullOrEmpty())
                                        {
                                            obj.Compositions = new List<Composition>();
                                            foreach (var composition in component.Compositions)
                                            {
                                                obj.Compositions.Add(composition.ToComposition());
                                            }
                                        }

                                        //// DataItems
                                        //if (component.DataItemCollection != null && !component.DataItemCollection.DataItems.IsNullOrEmpty())
                                        //{
                                        //    obj.DataItems = component.DataItemCollection.DataItems;
                                        //}

                                        //// Compositions
                                        //if (component.CompositionCollection != null && !component.CompositionCollection.Compositions.IsNullOrEmpty())
                                        //{
                                        //    obj.Compositions = component.CompositionCollection.Compositions;
                                        //}

                                        // Components
                                        if (component.ComponentCollection != null && !component.ComponentCollection.Components.IsNullOrEmpty())
                                        {
                                            obj.Components = component.ComponentCollection.Components;
                                        }

                                        Components.Add(obj);
                                    }
                                    else
                                    {
                                        // If no derived class is found then just add as base Component
                                        component.Type = child.Name;
                                        Components.Add(component.ToComponent());
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
